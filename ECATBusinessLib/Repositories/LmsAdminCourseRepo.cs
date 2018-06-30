﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Ecat.Data.Models.School;
using Ecat.Data.Models.User;
using Ecat.Business.Utilities;
using Ecat.Business.BbWs.BbCourse;
using Ecat.Business.BbWs.BbCourseMembership;
using Ecat.Business.BbWs.BbUser;
using Ecat.Data.Contexts;
using Ecat.Data.Models.Common;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.Canvas;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Static;
using Ecat.Business.Guards;
using Newtonsoft.Json.Linq;
using EntityState = System.Data.Entity.EntityState;

namespace Ecat.Business.Repositories
{
    public class CourseOps : ILmsAdminCourseOps
    {
        private readonly EFContextProvider<EcatContext> ctxManager;

        private readonly BbWsCnet _bbWs;

        //TODO: Update once we have production Canvas
        private readonly string canvasApiUrl = "https://lms.stag.af.edu/api/v1/";

        public ProfileFaculty Faculty { get; set; }

        public CourseOps(EFContextProvider<EcatContext> mainCtx, BbWsCnet bbWs)
        {
            ctxManager = mainCtx;
            _bbWs = bbWs;
        }

        public string Metadata
        {
            get
            {
                var efCtx = new EFContextProvider<LmsAdminMetadataCtx>();
                return efCtx.Metadata();
            }
        }

        public SaveResult SaveClientChanges(JObject saveBundle)
        {
            var guardian = new IsaGuard(ctxManager, Faculty.Person);
            ctxManager.BeforeSaveEntitiesDelegate += guardian.BeforeSaveEntities;
            return ctxManager.SaveChanges(saveBundle);
        }

        public async Task<List<Course>> GetAllCourses()
        {

            return await ctxManager.Context.Courses
                .Where(course => course.AcademyId == Faculty.AcademyId)
                .ToListAsync();
        }

        public async Task<List<WorkGroupModel>> GetCourseModels(int courseId)
        {
            var course = await ctxManager.Context.Courses
                .Where(crs => crs.Id == courseId)
                .Include(crse => crse.WorkGroups)
                .SingleAsync();

            var edLevel = StaticAcademy.AcadLookupById
                .Single(acad => acad.Key == course.AcademyId)
                .Value
                .MpEdLevel;

            var models = await ctxManager.Context.WgModels
                .Where(mdl => mdl.MpEdLevel == edLevel && mdl.IsActive)
                .ToListAsync();

            models.ForEach(mdl => mdl.WorkGroups = course.WorkGroups.Where(grp => grp.WgModelId == mdl.Id).ToList());

            return models;
        }

        public async Task<List<CategoryVO>> GetBbCategories()
        {
            var filter = new CategoryFilter
            {
                filterType = (int) CategoryFilterTpe.GetAllCourseCategory,
                filterTypeSpecified = true
            };
            var autoRetry = new Retrier<CategoryVO[]>();
            var categories = await autoRetry.Try(() => _bbWs.BbCourseCategories(filter), 3);

            return categories.ToList();
        }

        public async Task<CourseReconResult> ReconcileCourses()
        {
            //await GetProfile();

            var courseFilter = new CourseFilter
            {
                filterTypeSpecified = true,
                filterType = (int) CourseFilterType.LoadByCatId
            };

            if (Faculty != null)
            {
                var academy = StaticAcademy.AcadLookupById[Faculty.AcademyId];
                courseFilter.categoryIds = new[] {academy.BbCategoryId};
            }
            else
            {
                var ids = StaticAcademy.AcadLookupById.Select(acad => acad.Value.BbCategoryId).ToArray();
                courseFilter.categoryIds = ids;
            }

            var autoRetry = new Retrier<CourseVO[]>();
            var bbCoursesResult = await autoRetry.Try(() => _bbWs.BbCourses(courseFilter), 3);

            if (bbCoursesResult == null) throw new InvalidDataException("No Bb Responses received");

            var queryKnownCourses = ctxManager.Context.Courses.AsQueryable();

            queryKnownCourses = Faculty == null
                ? queryKnownCourses
                : queryKnownCourses.Where(crse => crse.AcademyId == Faculty.AcademyId);

            var knownCoursesIds = queryKnownCourses.Select(crse => crse.BbCourseId).ToList();

            var reconResult = new CourseReconResult
            {
                Id = Guid.NewGuid(),
                AcademyId = Faculty?.AcademyId,
                Courses = new List<Course>()
            };

            foreach (var nc in bbCoursesResult
                .Where(bbc => !knownCoursesIds.Contains(bbc.id))
                .Select(bbc => new Course
                {
                    BbCourseId = bbc.id,
                    AcademyId = Faculty.AcademyId,
                    Name = bbc.name,
                    StartDate = DateTime.Now,
                    GradDate = DateTime.Now.AddDays(25)
                }))
            {
                reconResult.NumAdded += 1;
                reconResult.Courses.Add(nc);
                ctxManager.Context.Courses.Add(nc);
            }

            await ctxManager.Context.SaveChangesAsync();

            foreach (var course in reconResult.Courses)
            {
                course.ReconResultId = reconResult.Id;
            }

            return reconResult;
        }

        public async Task<CourseReconResult> ReconcileCanvasCourses()
        {
            //await GetProfile();
            var academy = new Academy();
            if (Faculty != null)
            {
                academy = StaticAcademy.AcadLookupById[Faculty.AcademyId];
            }
            else
            {
                return null;
            }

            var reconResult = new CourseReconResult();

            var canvasLogin = await ctxManager.Context.CanvasLogins.Where(cl => cl.PersonId == Faculty.PersonId)
                .SingleOrDefaultAsync();

            if (canvasLogin.AccessToken == null)
            {
                reconResult.HasToken = false;
                return reconResult;
            }

            var client = new HttpClient();
            var apiAddr = new Uri(canvasApiUrl + "accounts/" + academy.CanvasAcctId + "/courses?per_page=1000");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + canvasLogin.AccessToken);

            var response = await client.GetAsync(apiAddr);

            var apiResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var coursesReturned = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CanvasCourse>>(apiResponse);
                var reconCourses = new List<Course>();

                if (coursesReturned == null || coursesReturned.Count == 0)
                {
                    return reconResult;
                }
                else
                {
                    var existingCourses =
                        await ctxManager.Context.Courses.Where(c => c.AcademyId == academy.Id).ToListAsync();
                    var existingIds = new List<string>();
                    //Bb course id is a string, Canvas course ids are numbers
                    existingCourses.ForEach(c => existingIds.Add(c.BbCourseId));

                    coursesReturned.ForEach(c =>
                    {
                        if (!existingIds.Contains(c.id.ToString()))
                        {
                            var newCourse = new Course();
                            newCourse.AcademyId = academy.Id;
                            newCourse.Name = c.name;
                            newCourse.BbCourseId = c.id.ToString();

                            if (c.start_at == null)
                            {
                                newCourse.StartDate = DateTime.Now;
                            }
                            else
                            {
                                newCourse.StartDate = DateTime.Parse(c.start_at);
                            }

                            if (c.end_at == null)
                            {
                                newCourse.GradDate = DateTime.Now.AddDays(30);
                            }
                            else
                            {
                                newCourse.GradDate = DateTime.Parse(c.end_at);
                            }

                            reconCourses.Add(newCourse);
                            ctxManager.Context.Courses.Add(newCourse);
                            reconResult.NumAdded += 1;
                        }
                    });

                    if (reconCourses.Any())
                    {
                        await ctxManager.Context.SaveChangesAsync();
                    }

                }

                reconResult.Id = Guid.NewGuid();
                reconResult.AcademyId = Faculty?.AcademyId;
                reconResult.Courses = reconCourses;
            }


            return reconResult;
        }

        public async Task<Course> GetAllCourseMembers(int courseId)
        {
            var query = await ctxManager.Context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => new
                {
                    c,
                    Students = c.Students
                        .Where(sic => !sic.IsDeleted)
                        .Select(sic => new
                        {
                            sic,
                            sic.Student,
                            sic.Student.Person
                        }),
                    Faculty = c.Faculty.Where(fic => !fic.IsDeleted)
                        .Select(fic => new
                        {
                            fic,
                            fic.FacultyProfile,
                            fic.FacultyProfile.Person
                        })
                })
                .SingleAsync();

            return query.c;
        }

        public async Task<MemReconResult> ReconcileCourseMembers(int courseId)
        {
            //await GetProfile();

            var ecatCourse = await ctxManager.Context.Courses
                .Where(crse => crse.Id == courseId)
                .Select(crse => new CourseReconcile
                {
                    Course = crse,
                    FacultyToReconcile = crse.Faculty.Select(fac => new UserReconcile
                    {
                        PersonId = fac.FacultyPersonId,
                        BbUserId = fac.FacultyProfile.Person.BbUserId,
                        CanDelete = !fac.FacSpComments.Any() &&
                                    !fac.FacSpResponses.Any() &&
                                    !fac.FacStratResponse.Any()
                    }).ToList(),
                    StudentsToReconcile = crse.Students.Select(sic => new UserReconcile
                    {
                        PersonId = sic.StudentPersonId,
                        BbUserId = sic.Student.Person.BbUserId,
                        CanDelete = !sic.WorkGroupEnrollments.Any()
                    }).ToList()
                }).SingleOrDefaultAsync();

            Contract.Assert(ecatCourse != null);

            var reconResult = new MemReconResult
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                AcademyId = Faculty?.AcademyId
            };

            var autoRetryCm = new Retrier<CourseMembershipVO[]>();

            var courseMemFilter = new MembershipFilter
            {
                filterTypeSpecified = true,
                filterType = (int) CrseMembershipFilterType.LoadByCourseId,
            };

            var bbCourseMems =
                await autoRetryCm.Try(() => _bbWs.BbCourseMembership(ecatCourse.Course.BbCourseId, courseMemFilter), 3);

            var existingCrseUserIds = ecatCourse.FacultyToReconcile
                .Select(fac => fac.BbUserId).ToList();

            existingCrseUserIds.AddRange(ecatCourse.StudentsToReconcile.Select(sic => sic.BbUserId));

            var newMembers = bbCourseMems
                .Where(cm => !existingCrseUserIds.Contains(cm.userId))
                .Where(cm => cm.available == true)
                .ToList();

            if (newMembers.Any())
            {
                //var queryCr = await autoRetryCr.Try(client.getCourseRolesAsync(bbCourseMems.Select(bbcm => bbcm.roleId).ToArray()), 3);

                //var bbCourseRoles = queryCr.@return.ToList();
                reconResult = await AddNewUsers(newMembers, reconResult);
                reconResult.NumAdded = newMembers.Count();
            }

            var usersBbIdsToRemove = existingCrseUserIds
                .Where(ecu => !bbCourseMems.Select(cm => cm.userId).Contains(ecu)).ToList();

            if (usersBbIdsToRemove.Any())
            {
                reconResult.RemovedIds = await RemoveOrFlagUsers(ecatCourse, usersBbIdsToRemove);
                reconResult.NumRemoved = reconResult.RemovedIds.Count();
            }

            return reconResult;
        }

        public async Task<MemReconResult> ReconcileCanvasCourseMems(int courseId)
        {
            //await GetProfile();

            var course = await ctxManager.Context.Courses.Where(c => c.Id == courseId)
                .Include(c => c.Students)
                .Include(c => c.Faculty)
                .SingleOrDefaultAsync();
            var reconResult = new MemReconResult();
            reconResult.Id = Guid.NewGuid();
            reconResult.AcademyId = Faculty?.AcademyId;
            reconResult.CourseId = courseId;
            reconResult.NumAdded = 0;
            reconResult.NumOfAccountCreated = 0;

            var canvasLogin = await ctxManager.Context.CanvasLogins.Where(cl => cl.PersonId == Faculty.PersonId)
                .SingleOrDefaultAsync();

            if (canvasLogin.AccessToken == null)
            {
                reconResult.HasToken = false;
                return reconResult;
            }

            var client = new HttpClient();
            var apiAddr = new Uri(canvasApiUrl + "courses/" + course.BbCourseId +
                                  "/enrollments?include[]=observed_users&per_page=1000");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + canvasLogin.AccessToken);

            var response = await client.GetAsync(apiAddr);

            var apiResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var enrollmentsReturned =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<List<CanvasEnrollment>>(apiResponse);

                if (enrollmentsReturned == null || enrollmentsReturned.Count == 0)
                {
                    return reconResult;
                }
                else
                {
                    var newEnrolls = new List<CanvasEnrollment>();
                    var removedStudents = new List<StudentInCourse>();
                    var removedFaculty = new List<FacultyInCourse>();
                    //var retStuEnrollIds = new List<string>();
                    //var retFacEnrollIds = new List<string>();
                    var retStuUserIds = new List<string>();
                    var retFacUserIds = new List<string>();

                    enrollmentsReturned.ForEach(ce =>
                    {
                        if (MpRoleTransform.CanvasRoleToEcat(ce.type) == MpInstituteRoleId.Student)
                        {
                            retStuUserIds.Add(ce.user_id.ToString());
                            var stuEnrollExists = course.Students.Where(sic => sic.BbCourseMemId == ce.id.ToString())
                                .Single();

                            //TODO: Will students disenrolled in iGecko/SIS show as inactive or be removed from the course in Canvas completely? Update accordingly
                            if (ce.enrollment_state == "inactive")
                            {
                                if (stuEnrollExists != null && !stuEnrollExists.IsDeleted)
                                {
                                    removedStudents.Add(stuEnrollExists);
                                }
                            }

                            //retStuEnrollIds.Add(ce.id.ToString());

                            if (ce.enrollment_state == "active")
                            {
                                if (stuEnrollExists == null)
                                {
                                    newEnrolls.Add(ce);
                                }

                                if (stuEnrollExists != null && stuEnrollExists.IsDeleted)
                                {
                                    stuEnrollExists.IsDeleted = false;
                                    ctxManager.Context.Entry(stuEnrollExists).State =
                                        System.Data.Entity.EntityState.Modified;
                                    reconResult.NumAdded++;
                                }
                            }
                        }

                        if (MpRoleTransform.CanvasRoleToEcat(ce.type) == MpInstituteRoleId.Faculty)
                        {
                            retFacUserIds.Add(ce.user_id.ToString());
                            var facEnrollExists = course.Faculty.Where(fic => fic.BbCourseMemId == ce.id.ToString())
                                .Single();

                            //TODO: Will faculty disenrolled in iGecko/SIS show as inactive or be removed from the course in Canvas completely? Update accordingly
                            if (ce.enrollment_state == "inactive")
                            {
                                if (facEnrollExists != null && !facEnrollExists.IsDeleted)
                                {
                                    removedFaculty.Add(facEnrollExists);
                                }
                            }

                            //retFacEnrollIds.Add(ce.id.ToString());

                            if (ce.enrollment_state == "active")
                            {
                                if (facEnrollExists == null)
                                {
                                    newEnrolls.Add(ce);
                                }

                                if (facEnrollExists != null && facEnrollExists.IsDeleted)
                                {
                                    facEnrollExists.IsDeleted = false;
                                    ctxManager.Context.Entry(facEnrollExists).State =
                                        System.Data.Entity.EntityState.Modified;
                                    reconResult.NumAdded++;
                                }
                            }
                        }
                    });

                    var students = new List<Person>();
                    var faculty = new List<Person>();

                    if (retStuUserIds.Any())
                    {
                        students = await ctxManager.Context.People.Where(p => retStuUserIds.Contains(p.BbUserId))
                            .Include(p => p.Student).ToListAsync();
                    }

                    if (retFacUserIds.Any())
                    {
                        faculty = await ctxManager.Context.People.Where(p => retFacUserIds.Contains(p.BbUserId))
                            .Include(p => p.Faculty).ToListAsync();
                    }

                    if (newEnrolls.Any())
                    {
                        var newEcatAccts = new List<Person>();
                        var newStuEnrolls = new List<StudentInCourse>();
                        var newFacEnrolls = new List<FacultyInCourse>();
                        newEnrolls.ForEach(ne =>
                        {
                            var ecatAcct = new Person();
                            if (MpRoleTransform.CanvasRoleToEcat(ne.type) == MpInstituteRoleId.Student)
                            {
                                ecatAcct = students.Where(s => s.BbUserId == ne.user_id.ToString()).Single();
                            }

                            if (MpRoleTransform.CanvasRoleToEcat(ne.type) == MpInstituteRoleId.Faculty)
                            {
                                ecatAcct = faculty.Where(s => s.BbUserId == ne.user_id.ToString()).Single();
                            }

                            //if a user has multiple section enrollments in the same course in Canvas they come back from the API each as different enrollment objects
                            if (ecatAcct.BbUserId == null)
                            {
                                ecatAcct.BbUserId = ne.user_id.ToString();
                                ecatAcct.BbUserName = ne.user.login_id;
                                ecatAcct.Email = ne.user.login_id;
                                var nameSplit = ne.user.sortable_name.Split(',');
                                ecatAcct.LastName = nameSplit[0];
                                ecatAcct.FirstName = nameSplit[1].TrimStart(' ');
                                ecatAcct.IsActive = true;
                                ecatAcct.MpGender = MpGender.Unk;
                                ecatAcct.MpAffiliation = MpAffiliation.Unk;
                                ecatAcct.MpComponent = MpComponent.Unk;
                                ecatAcct.RegistrationComplete = false;
                                ecatAcct.MpPaygrade = MpPaygrade.Unk;
                                ecatAcct.ModifiedById = Faculty?.PersonId;
                                ecatAcct.ModifiedDate = DateTime.Now;
                                ecatAcct.AvatarLocation = ne.user.avatar_url;

                                ecatAcct.MpInstituteRole = MpRoleTransform.CanvasRoleToEcat(ne.type);

                                newEcatAccts.Add(ecatAcct);
                            }

                            if (ecatAcct.MpInstituteRole == MpInstituteRoleId.Student)
                            {
                                if (ecatAcct.Student == null)
                                {
                                    ecatAcct.Student = new ProfileStudent();
                                }

                                students.Add(ecatAcct);

                                var newEnroll = new StudentInCourse();
                                newEnroll.StudentPersonId = ecatAcct.PersonId;
                                newEnroll.CourseId = courseId;
                                newEnroll.BbCourseMemId = ne.id.ToString();

                                newStuEnrolls.Add(newEnroll);
                            }

                            if (ecatAcct.MpInstituteRole == MpInstituteRoleId.Faculty ||
                                ecatAcct.MpInstituteRole == MpInstituteRoleId.CourseAdmin)
                            {
                                if (ecatAcct.Faculty == null)
                                {
                                    ecatAcct.Faculty = new ProfileFaculty();
                                }

                                ecatAcct.Faculty.AcademyId = course.AcademyId;
                                faculty.Add(ecatAcct);

                                var newEnroll = new FacultyInCourse();
                                newEnroll.FacultyPersonId = ecatAcct.PersonId;
                                newEnroll.CourseId = courseId;
                                newEnroll.BbCourseMemId = ne.id.ToString();

                                newFacEnrolls.Add(newEnroll);
                            }

                        });

                        if (newEcatAccts.Any())
                        {
                            ctxManager.Context.People.AddRange(newEcatAccts);
                            reconResult.NumOfAccountCreated = newEcatAccts.Count;
                        }

                        if (newStuEnrolls.Any())
                        {
                            ctxManager.Context.StudentInCourses.AddRange(newStuEnrolls);
                            reconResult.NumAdded += newStuEnrolls.Count;
                        }

                        if (newFacEnrolls.Any())
                        {
                            ctxManager.Context.FacultyInCourses.AddRange(newFacEnrolls);
                            reconResult.NumAdded += newFacEnrolls.Count;
                        }
                    }

                    //is there a scenario where someone has a Person record, but not a profile?
                    var stusNeedProfiles = students.Where(p => p.Student == null).ToList();
                    stusNeedProfiles.ForEach(u =>
                    {
                        u.Student = new ProfileStudent();
                        u.Student.PersonId = u.PersonId;
                        ctxManager.Context.Students.Add(u.Student);
                    });

                    var facsNeedProfiles = faculty.Where(p => p.Faculty == null).ToList();
                    facsNeedProfiles.ForEach(u =>
                    {
                        u.Faculty = new ProfileFaculty();
                        u.Faculty.PersonId = u.PersonId;
                        u.Faculty.AcademyId = course.AcademyId;
                        ctxManager.Context.Faculty.Add(u.Faculty);
                    });

                    //TODO: Will students disenrolled in iGecko/SIS show as inactive or be removed from the course in Canvas completely? Update accordingly
                    //var removedFaculty = course.Faculty.Where(fic => !retFacEnrollIds.Contains(fic.BbCourseMemId) && !fic.isDeleted).ToList();
                    //var removedStudents = course.Students.Where(sic => !retStuEnrollIds.Contains(sic.BbCourseMemId) && !sic.isDeleted).ToList();

                    int numRemoved = 0;
                    if (removedFaculty.Any())
                    {
                        removedFaculty.ForEach(fic =>
                        {
                            fic.IsDeleted = true;
                            ctxManager.Context.Entry(fic).State = System.Data.Entity.EntityState.Modified;
                            numRemoved++;
                        });
                    }

                    if (removedStudents.Any())
                    {
                        removedStudents.ForEach(sic =>
                        {
                            sic.IsDeleted = true;
                            ctxManager.Context.Entry(sic).State = System.Data.Entity.EntityState.Modified;
                            numRemoved++;
                        });

                        //need to delete studentingroup records so we don't mess up the group management screens
                        var studIdList = removedStudents.Select(sic => sic.StudentPersonId).ToList();
                        var csigList = await ctxManager.Context.StudentInGroups.Where(csig =>
                                studIdList.Contains(csig.StudentId) && csig.CourseId == course.Id && !csig.IsDeleted)
                            .Include(csig => csig.WorkGroup)
                            .Include(csig => csig.AssesseeSpResponses)
                            .Include(csig => csig.AssesseeStratResponse)
                            .Include(csig => csig.AssessorSpResponses)
                            .Include(csig => csig.AssessorStratResponse)
                            .Include(csig => csig.AuthorOfComments)
                            .Include(csig => csig.RecipientOfComments)
                            .Include(csig => csig.FacultyComment)
                            .Include(csig => csig.FacultySpResponses)
                            .Include(csig => csig.FacultyStrat)
                            .ToListAsync();
                        var groupIdList = csigList.Select(csig => csig.WorkGroupId).Distinct().ToList();
                        //var commFlags = await ctxManager.Context.StudSpCommentFlag.Where(flag => groupIdList.Contains(flag.WorkGroupId) && (studIdList.Contains(flag.AuthorPersonId) || studIdList.Contains(flag.RecipientPersonId))).ToListAsync();
                        var authorFlags = await ctxManager.Context.StudSpCommentFlags.Where(flag =>
                                groupIdList.Contains(flag.WorkGroupId) && studIdList.Contains(flag.AuthorPersonId))
                            .ToListAsync();
                        var recipFlags = await ctxManager.Context.StudSpCommentFlags.Where(flag =>
                                groupIdList.Contains(flag.WorkGroupId) && studIdList.Contains(flag.RecipientPersonId))
                            .ToListAsync();
                        var facFlags = await ctxManager.Context.FacSpCommentFlags.Where(flag =>
                                groupIdList.Contains(flag.WorkGroupId) && studIdList.Contains(flag.RecipientPersonId))
                            .ToListAsync();

                        //if (commFlags.Any()) { ctxManager.Context.StudSpCommentFlag.RemoveRange(commFlags); }
                        //if (facFlags.Any()) { ctxManager.Context.facSpCommentsFlag.RemoveRange(facFlags); }

                        studIdList.ForEach(id =>
                        {
                            var groupMems = csigList.Where(gm => gm.StudentId == id).ToList();
                            if (groupMems.Any())
                            {
                                groupMems.ForEach(gm =>
                                {
                                    if (gm.WorkGroup.MpSpStatus != MpSpStatus.Published)
                                    {
                                        if (gm.AssesseeSpResponses.Any())
                                        {
                                            ctxManager.Context.SpResponses.RemoveRange(gm.AssesseeSpResponses);
                                        }

                                        if (gm.AssesseeStratResponse.Any())
                                        {
                                            ctxManager.Context.SpStratResponses.RemoveRange(gm.AssesseeStratResponse);
                                        }

                                        if (gm.AssessorSpResponses.Any())
                                        {
                                            ctxManager.Context.SpResponses.RemoveRange(gm.AssessorSpResponses);
                                        }

                                        if (gm.AssessorStratResponse.Any())
                                        {
                                            ctxManager.Context.SpStratResponses.RemoveRange(gm.AssessorStratResponse);
                                        }

                                        if (gm.AuthorOfComments.Any())
                                        {
                                            var gmAuthorFlags = authorFlags.Where(flag =>
                                                flag.AuthorPersonId == gm.StudentId &&
                                                flag.WorkGroupId == gm.WorkGroupId).ToList();
                                            ctxManager.Context.StudSpCommentFlags.RemoveRange(gmAuthorFlags);
                                            ctxManager.Context.StudSpComments.RemoveRange(gm.AuthorOfComments);
                                        }

                                        if (gm.RecipientOfComments.Any())
                                        {
                                            var gmRecipFlags = recipFlags.Where(flag =>
                                                flag.RecipientPersonId == gm.StudentId &&
                                                flag.WorkGroupId == gm.WorkGroupId).ToList();
                                            ctxManager.Context.StudSpCommentFlags.RemoveRange(gmRecipFlags);
                                            ctxManager.Context.StudSpComments.RemoveRange(gm.RecipientOfComments);
                                        }

                                        if (gm.FacultyComment != null)
                                        {
                                            var gmFacFlag = facFlags.Where(flag =>
                                                flag.RecipientPersonId == gm.StudentId &&
                                                flag.WorkGroupId == gm.WorkGroupId).Single();
                                            ctxManager.Context.FacSpCommentFlags.Remove(gmFacFlag);
                                            ctxManager.Context.FacSpComments.Remove(gm.FacultyComment);
                                        }

                                        if (gm.FacultySpResponses.Any())
                                        {
                                            ctxManager.Context.FacSpResponses.RemoveRange(gm.FacultySpResponses);
                                        }

                                        if (gm.FacultyStrat != null)
                                        {
                                            ctxManager.Context.FacStratResponses.Remove(gm.FacultyStrat);
                                        }


                                        ctxManager.Context.StudentInGroups.Remove(gm);
                                    }

                                });
                            }
                        });

                    }

                    reconResult.NumRemoved = numRemoved;
                }

                if (reconResult.NumOfAccountCreated > 0 || reconResult.NumAdded > 0 || reconResult.NumRemoved > 0)
                {
                    await ctxManager.Context.SaveChangesAsync();
                }
            }

            return reconResult;
        }

        private async Task<MemReconResult> AddNewUsers(IEnumerable<CourseMembershipVO> bbCmsVo,
            MemReconResult reconResult)
        {
            var bbCms = bbCmsVo.ToList();
            var bbCmUserIds = bbCms.Select(bbcm => bbcm.userId).ToList();

            var usersWithAccount = await ctxManager.Context.People
                .Where(p => bbCmUserIds.Contains(p.BbUserId))
                .Select(p => new
                {
                    p.BbUserId,
                    p.PersonId,
                    p.MpInstituteRole
                })
                .ToListAsync();

            var accountsNeedToCreate =
                bbCms.Where(cm => !usersWithAccount.Select(uwa => uwa.BbUserId).Contains(cm.userId)).ToList();

            if (accountsNeedToCreate.Any())
            {
                var userFilter = new UserFilter
                {
                    filterTypeSpecified = true,
                    filterType = (int) UserFilterType.UserByIdWithAvailability,
                    available = true,
                    availableSpecified = true,
                    id = accountsNeedToCreate.Select(bbAccount => bbAccount.userId).ToArray()
                };

                var autoRetryUsers = new Retrier<UserVO[]>();
                var bbUsers = await autoRetryUsers.Try(() => _bbWs.BbCourseUsers(userFilter), 3);
                var courseMems = bbCms;

                if (bbUsers != null)
                {

                    var users = bbUsers.Select(bbu =>
                    {
                        var cm = courseMems.First(bbcm => bbcm.userId == bbu.id);
                        return new Person
                        {
                            MpInstituteRole = MpRoleTransform.BbWsRoleToEcat(cm.roleId),
                            BbUserId = bbu.id,
                            BbUserName = bbu.name,
                            Email = $"{cm.id}-{bbu.extendedInfo.emailAddress}",
                            IsActive = true,
                            LastName = bbu.extendedInfo.familyName,
                            FirstName = bbu.extendedInfo.givenName,
                            MpGender = MpGender.Unk,
                            MpAffiliation = MpAffiliation.Unk,
                            MpComponent = MpComponent.Unk,
                            RegistrationComplete = false,
                            MpPaygrade = MpPaygrade.Unk,
                            ModifiedById = Faculty.PersonId,
                            ModifiedDate = DateTime.Now
                        };
                    }).ToList();

                    foreach (var user in users)
                    {
                        ctxManager.Context.People.Add(user);

                    }

                    reconResult.NumOfAccountCreated = await ctxManager.Context.SaveChangesAsync();

                    foreach (var user in users)
                    {
                        usersWithAccount.Add(new
                        {
                            user.BbUserId,
                            user.PersonId,
                            user.MpInstituteRole
                        });

                        switch (user.MpInstituteRole)
                        {
                            case MpInstituteRoleId.Faculty:
                                user.Faculty = new ProfileFaculty
                                {
                                    PersonId = user.PersonId,
                                    AcademyId = reconResult.AcademyId,
                                    HomeStation = StaticAcademy.AcadLookupById[reconResult.AcademyId].Base.ToString(),
                                    IsReportViewer = false,
                                    IsCourseAdmin = false
                                };
                                break;
                            case MpInstituteRoleId.Student:
                                user.Student = new ProfileStudent
                                {
                                    PersonId = user.PersonId
                                };
                                break;
                            default:
                                user.Student = new ProfileStudent
                                {
                                    PersonId = user.PersonId
                                };
                                break;
                        }
                    }

                    await ctxManager.Context.SaveChangesAsync();
                }
            }

            reconResult.Students = usersWithAccount
                .Where(ecm =>
                {
                    var bbCM = bbCmsVo.First(cm => cm.userId == ecm.BbUserId);
                    return MpRoleTransform.BbWsRoleToEcat(bbCM.roleId) != MpInstituteRoleId.Faculty;
                })
                .Select(ecm => new StudentInCourse
                {
                    StudentPersonId = ecm.PersonId,
                    CourseId = reconResult.CourseId,
                    ReconResultId = reconResult.Id,
                    BbCourseMemId = bbCms.First(bbcm => bbcm.userId == ecm.BbUserId).id
                }).ToList();

            reconResult.Faculty = usersWithAccount
                .Where(ecm =>
                {
                    var bbCM = bbCmsVo.First(cm => cm.userId == ecm.BbUserId);
                    return MpRoleTransform.BbWsRoleToEcat(bbCM.roleId) == MpInstituteRoleId.Faculty;
                })
                .Select(ecm => new FacultyInCourse
                {
                    FacultyPersonId = ecm.PersonId,
                    CourseId = reconResult.CourseId,
                    ReconResultId = reconResult.Id,
                    BbCourseMemId = bbCms.First(bbcm => bbcm.userId == ecm.BbUserId).id
                }).ToList();

            var neededFacultyProfiles = usersWithAccount.Where(ecm =>
            {
                var bbCM = bbCmsVo.First(cm => cm.userId == ecm.BbUserId);
                return MpRoleTransform.BbWsRoleToEcat(bbCM.roleId) == MpInstituteRoleId.Faculty;
            }).Select(ecm => ecm.PersonId);

            var existingFacultyProfiles = ctxManager.Context.Faculty
                .Where(fac => neededFacultyProfiles.Contains(fac.PersonId))
                .Select(fac => fac.PersonId);

            var newFacultyProfiles = neededFacultyProfiles
                .Where(id => !existingFacultyProfiles.Contains(id))
                .Select(id => new ProfileFaculty
                {
                    PersonId = id,
                    AcademyId = reconResult.AcademyId,
                    HomeStation = StaticAcademy.AcadLookupById[reconResult.AcademyId].Base.ToString(),
                    IsReportViewer = false,
                    IsCourseAdmin = false
                });

            var neededStudentProfiles = usersWithAccount.Where(ecm =>
            {
                var bbCM = bbCmsVo.First(cm => cm.userId == ecm.BbUserId);
                return MpRoleTransform.BbWsRoleToEcat(bbCM.roleId) != MpInstituteRoleId.Faculty;
            }).Select(ecm => ecm.PersonId);

            var existingStudentProfiles = ctxManager.Context.Students
                .Where(stud => neededStudentProfiles.Contains(stud.PersonId))
                .Select(stud => stud.PersonId);

            var newStudentProfiles = neededStudentProfiles
                .Where(id => !existingStudentProfiles.Contains(id))
                .Select(id => new ProfileStudent
                {
                    PersonId = id,
                });

            if (newFacultyProfiles.Any()) ctxManager.Context.Faculty.AddRange(newFacultyProfiles);
            if (newStudentProfiles.Any()) ctxManager.Context.Students.AddRange(newStudentProfiles);

            if (reconResult.Students.Any()) ctxManager.Context.StudentInCourses.AddRange(reconResult.Students);

            if (reconResult.Faculty.Any()) ctxManager.Context.FacultyInCourses.AddRange(reconResult.Faculty);

            reconResult.NumAdded = reconResult.Faculty.Any() || reconResult.Students.Any()
                ? await ctxManager.Context.SaveChangesAsync()
                : 0;

            return reconResult;
        }

        private async Task<List<int>> RemoveOrFlagUsers(CourseReconcile courseToReconcile,
            IEnumerable<string> bbIdsToRemove)
        {
            var idsRemoved = new List<int>();

            foreach (var studReconcile in courseToReconcile.StudentsToReconcile.Where(str =>
                bbIdsToRemove.Contains(str.BbUserId)))
            {
                idsRemoved.Add(studReconcile.PersonId);
                var sic = new StudentInCourse
                {
                    StudentPersonId = studReconcile.PersonId,
                    CourseId = courseToReconcile.Course.Id
                };

                if (studReconcile.CanDelete)
                {
                    ctxManager.Context.Entry(sic).State = System.Data.Entity.EntityState.Deleted;
                }
                else
                {
                    sic.DeletedDate = DateTime.Now;
                    sic.DeletedById = Faculty.PersonId;
                    sic.IsDeleted = true;
                    ctxManager.Context.Entry(sic).State = System.Data.Entity.EntityState.Modified;
                }
            }

            foreach (var facReconcile in courseToReconcile.FacultyToReconcile.Where(str =>
                bbIdsToRemove.Contains(str.BbUserId)))
            {
                idsRemoved.Add(facReconcile.PersonId);
                var fic = new FacultyInCourse
                {
                    FacultyPersonId = facReconcile.PersonId,
                    CourseId = courseToReconcile.Course.Id
                };

                if (facReconcile.CanDelete)
                {
                    ctxManager.Context.Entry(fic).State = System.Data.Entity.EntityState.Deleted;
                }
                else
                {
                    fic.DeletedDate = DateTime.Now;
                    fic.DeletedById = Faculty.PersonId;
                    fic.IsDeleted = true;
                    ctxManager.Context.Entry(fic).State = System.Data.Entity.EntityState.Modified;
                }
            }

            await ctxManager.Context.SaveChangesAsync();

            return idsRemoved;
        }
    }

}

