using System;
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
using Ecat.Business.Business;
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

        //New Canvas API Calls
        public async Task<CourseReconResult> PollCanvasCourses()
        {
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

            if (canvasLogin?.AccessToken == null)
            {
                reconResult.HasToken = false;
                return reconResult;

            }

            var response = new HttpResponseMessage();
            var apiResource= "accounts/" + academy.CanvasAcctId + "/courses?per_page=1000";

            try
            {
                response = await CanvasOps.GetResponse(Faculty.PersonId, apiResource, canvasLogin);
                reconResult.HasToken = true;
            }
            catch (HttpRequestException e)
            {
                var exception = e;
            }

            var apiResponse = await response.Content.ReadAsStringAsync();

            var coursesReturned = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CanvasCourse>>(apiResponse);

            if (coursesReturned == null || coursesReturned.Count == 0)
            {
                return reconResult;
            }

            var existingCourses =
                await ctxManager.Context.Courses.Where(c => c.AcademyId == academy.Id).ToListAsync();

            var reconCourses = CanvasBusinessLogic.ReconcileCourses(coursesReturned, existingCourses, academy.Id);

            if (reconCourses.Any())
            {
                reconCourses.ForEach(course => ctxManager.Context.Courses.Add((course)));
                await ctxManager.Context.SaveChangesAsync();
            }

            reconResult.NumAdded = reconCourses.Count();
            reconResult.Id = Guid.NewGuid();
            reconResult.AcademyId = Faculty?.AcademyId;
            reconResult.Courses = reconCourses;

            return reconResult;
        }

        public async Task<CourseDetailsReconResult> PollCanvasCourseDetails(int courseId)
        {

            var bbCourseId = await ctxManager.Context.Courses.Where(course => course.Id == courseId)
                .Select(course => course.BbCourseId).SingleOrDefaultAsync();

            var reconResult = new CourseDetailsReconResult
            {
                Id = Guid.NewGuid(),
                AcademyId = Faculty?.AcademyId,
                NumAdded = 0,
                NumRemoved = 0,
                HasToken = true,
                IsAuthorized = true,
                GroupMemReconSuccess = false,
                GroupReconSuccess = false,
                MemReconSuccess = false,
            };

            var canvasLogin = await ctxManager.Context.CanvasLogins.Where(cl => cl.PersonId == Faculty.PersonId)
                .SingleOrDefaultAsync();

            if (canvasLogin?.AccessToken == null)
            {
                reconResult.HasToken = false;
                return reconResult;

            }

            //Get List of users in Canvas Course
            var apiResourceSection = ("courses/" + bbCourseId + "/sections?per_page=1000&include[]=students&include[]=enrollments");
            var apiResourceFaculty = ("courses/" + bbCourseId + "/enrollments?include[]=observed_users&per_page=1000&type[]=TeacherEnrollment");

            var apiResponses = new List<HttpResponseMessage>();
            
            var sectionResponse = CanvasOps.GetResponse(Faculty.PersonId, apiResourceSection, canvasLogin);
            var facultyResponse = CanvasOps.GetResponse(Faculty.PersonId, apiResourceFaculty, canvasLogin);

            try
            {
                await Task.WhenAll(sectionResponse, facultyResponse);
            }

            catch (Exception e)
            {
                var exception = e;
            }
   

            apiResponses.Add(sectionResponse.Result);
            apiResponses.Add(facultyResponse.Result);
            var responsesThatHaveErrors = apiResponses.Where(response => response.IsSuccessStatusCode == false).ToList();

            if (responsesThatHaveErrors.Any())
            {
                var errorMessage = "There was an error with one or more calls to the Canvas API" + Environment.NewLine;
                responsesThatHaveErrors.ForEach(resp =>
                    {
                        errorMessage += resp.ReasonPhrase.ToString() + Environment.NewLine;
                    });
                reconResult.ErrorMessage = errorMessage;
                return reconResult;
            }
     
            var sectionApiResult = await sectionResponse.Result.Content.ReadAsStringAsync();
            var sections = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CanvasSection>>(sectionApiResult);

            var facultyApiResult = await facultyResponse.Result.Content.ReadAsStringAsync();
            var facultyEnrollments = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CanvasEnrollment>>(facultyApiResult);

            var studentEnrollments = sections.SelectMany(srr => srr.students).ToList();

            reconResult.CourseMemberReconResult = await SyncCanvasCourseEnrollments(courseId, studentEnrollments, facultyEnrollments);
            reconResult.MemReconSuccess = true;

            reconResult.GroupReconResult = await SyncCanvasSections(courseId, sections);
            reconResult.GroupReconSuccess = true;

            reconResult.GroupMemReconResults = await SyncCanvasSectionMembers(courseId, sections);
            reconResult.GroupMemReconSuccess = true;

            return reconResult;
        }

        private async Task<List<GroupMemReconResult>> SyncCanvasSectionMembers(int courseId, List<CanvasSection> sectionResultsReturned)
        {
            var courseWithWorkGroups = await ctxManager.Context.Courses
                .Where(crse => crse.Id == courseId)
                .Select(crse => new WorkgroupAndMemberReconcile
                {
                    CrseId = crse.Id,
                    BbCrseId = crse.BbCourseId,
                    WorkGroups = crse.WorkGroups
                        .Where(wg => wg.MpSpStatus != MpSpStatus.Published)
                        .Where(wg => wg.MpCategory == MpGroupCategory.Wg1)
                        .Select(wg => new WorkgroupReconcile
                        {
                            WgId = wg.WorkGroupId,
                            BbWgId = wg.BbGroupId,
                            Category = wg.MpCategory,
                            Name = wg.DefaultName,
                            HasMembers = wg.GroupMembers.Any(),
                            NeedToAdd = false,
                            Members = wg.GroupMembers.Select(gm => new WorkgroupMemberReconcile
                            {
                                WorkGroupId = wg.WorkGroupId,
                                StudentId = gm.StudentId,
                                IsDeleted = gm.IsDeleted,
                                BbUserId = gm.BbCrseStudGroupId,
                                IsMoving = false,
                                NewEnrollment = false,
                                RemoveEnrollment = false,
                                HasChildren = gm.AuthorOfComments.Any() ||
                                              gm.AssesseeSpResponses.Any() ||
                                              gm.AssessorSpResponses.Any() ||
                                              gm.AssesseeStratResponse.Any() ||
                                              gm.AssessorStratResponse.Any() ||
                                              gm.RecipientOfComments.Any()
                            }).ToList()
                        }).ToList()
                })
                .FirstAsync();

            var groupMemReconResults = new List<GroupMemReconResult>();
            var workGroups = courseWithWorkGroups.WorkGroups.ToList();
            workGroups.ForEach(wg =>
            {
                var newGroupMemReconResult = new GroupMemReconResult
                {
                    Id = Guid.NewGuid(),
                    AcademyId = Faculty?.AcademyId,
                    CourseId = courseWithWorkGroups.CrseId,
                    WorkGroupId = wg.WgId,
                    WorkGroupName = wg.Name,
                    GroupType = wg.Category,
                    GroupMembers = new List<CrseStudentInGroup>()
                };

                groupMemReconResults.Add(newGroupMemReconResult);
            });

           

            if (sectionResultsReturned == null)
            {
                var noCanvasSectionsReturnedResult = new GroupMemReconResult();
                noCanvasSectionsReturnedResult.HasToken = true;
                groupMemReconResults.Clear();
                groupMemReconResults.Add(noCanvasSectionsReturnedResult);
                return groupMemReconResults;
            }

            //Create map of everything that needs to be done for each member add/delete/move
            courseWithWorkGroups =
                CanvasBusinessLogic.ReconcileGroupMembers(courseId, courseWithWorkGroups, sectionResultsReturned);

            var membersToAdd = courseWithWorkGroups.WorkGroups.SelectMany(wg => wg.Members).Where(mem => mem.NewEnrollment).ToList();
            var membersToDelete = courseWithWorkGroups.WorkGroups.SelectMany(wg => wg.Members).Where(mem => mem.RemoveEnrollment).ToList();
            var membersToMove = courseWithWorkGroups.WorkGroups.SelectMany(wg => wg.Members).Where(mem => mem.IsMoving).ToList();

            var sectionsWithMembersToAdd = courseWithWorkGroups.WorkGroups
                .Where(wg => wg.Members.Any(member => member.NewEnrollment)).ToList();

            var workGroupsWithMembersToDelete = courseWithWorkGroups.WorkGroups
                .Where(wg => wg.Members.Any(member => member.RemoveEnrollment)).ToList();

            if (workGroupsWithMembersToDelete.Any())
            {
                groupMemReconResults = await RemoveWorkgroupMembers(groupMemReconResults, workGroupsWithMembersToDelete);
            }

            if (sectionsWithMembersToAdd.Any())
            {
                groupMemReconResults = await AddGroupMembers(groupMemReconResults, courseWithWorkGroups, sectionsWithMembersToAdd, courseId);
            }

            return groupMemReconResults;
        }

        private async Task<List<GroupMemReconResult>> RemoveWorkgroupMembers(List<GroupMemReconResult> groupMemReconResults, List<WorkgroupReconcile> workGroupsWithMembersToDelete)
        {

            foreach (var wg in workGroupsWithMembersToDelete)
            {
                var groupMemReconResult = groupMemReconResults.First(gmrr => gmrr.WorkGroupId == wg.WgId);
                var memberIdList = wg.Members.Where(mem => mem.RemoveEnrollment).Select(mem => mem.StudentId).ToList();

                var exisitingStudentInGroup = await
                    ctxManager.Context.StudentInGroups.Where(sig =>
                            memberIdList.Contains(sig.StudentId) &&
                            sig.WorkGroupId == wg.WgId)
                        .ToListAsync();

                foreach (var csig in exisitingStudentInGroup)
                {
                    RepoUtilities.RemoveAllGroupMembershipData(ctxManager, csig.StudentId, csig.WorkGroupId);
                    ctxManager.Context.Entry(csig).State = System.Data.Entity.EntityState.Deleted;

                }

                groupMemReconResult.NumRemoved = exisitingStudentInGroup.Count;
            }
           

            await ctxManager.Context.SaveChangesAsync();

            return groupMemReconResults;
        }

        private async Task<List<GroupMemReconResult>> AddGroupMembers(List<GroupMemReconResult> groupMemReconResults, WorkgroupAndMemberReconcile courseWithWorkGroups, List<WorkgroupReconcile> sectionsWithMembersToAdd, int courseId)
        {
            
            var additions = new List<CrseStudentInGroup>();

            var studentsInCourse = await ctxManager.Context.StudentInCourses
                .Where(stud => stud.CourseId == courseId)
                .Select(stud => new
                {
                    stud.BbCourseMemId,
                    stud.Student.Person.BbUserId,
                    stud.Student.PersonId
                })
                .ToListAsync();

            sectionsWithMembersToAdd.ForEach(section =>
            {
                var groupMemReconResult = groupMemReconResults.First(gmrr => gmrr.WorkGroupId == section.WgId);

                var membersToAdd = section.Members.Where(mem => mem.NewEnrollment).ToList();

                membersToAdd.ForEach(mta =>
                {
                    var student = studentsInCourse.SingleOrDefault(stud => stud.BbUserId == mta.BbUserId);

                    var newStudentInGroup = new CrseStudentInGroup
                    {
                        StudentId = student.PersonId,
                        CourseId = courseWithWorkGroups.CrseId,
                        WorkGroupId = section.WgId,
                        HasAcknowledged = false,
                        BbCrseStudGroupId = mta.BbUserId,                
                        IsDeleted = false,
                        DeletedDate = null,
                        DeletedById = null,
                        ModifiedById = Faculty.PersonId,
                        ModifiedDate = DateTime.Now,
                        ReconResultId = groupMemReconResult.Id,


                    };
                    groupMemReconResult.NumAdded += 1;
                    groupMemReconResult.GroupMembers.Add(newStudentInGroup);
                    additions.Add(newStudentInGroup);
                });  

                groupMemReconResults.Add(groupMemReconResult);
            });

            ctxManager.Context.StudentInGroups.AddRange(additions);
            await ctxManager.Context.SaveChangesAsync();

            return groupMemReconResults;
        }

        private async Task<MemReconResult> SyncCanvasCourseEnrollments(int courseId, List<CanvasUser> studentCanvasUsers, List<CanvasEnrollment> faCanvasEnrollments )
        {

            var courseMemReconResult = new MemReconResult
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                AcademyId = Faculty?.AcademyId,

            };
            //if a user has multiple section enrollments in the same course in Canvas they come back from the API each as different enrollment objects
            //Get current list of students/faculty in Course in Database
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
                                    !fac.FacStratResponse.Any(),
                        IsMarkedDeleted = fac.IsDeleted,
                        RemoveEnrollment = false,
                        FlagDeleted = false,
                        UnFlagDeleted = false,
                        NewEnrollment = false,
                    }).ToList(),
                    StudentsToReconcile = crse.Students.Select(sic => new UserReconcile
                    {
                        PersonId = sic.StudentPersonId,
                        BbUserId = sic.Student.Person.BbUserId,
                        CanDelete = !sic.WorkGroupEnrollments.Any(),
                        IsMarkedDeleted = sic.IsDeleted,
                        RemoveEnrollment = false,
                        FlagDeleted = false,
                        UnFlagDeleted = false,
                        NewEnrollment = false,
                    }).ToList()
                }).SingleOrDefaultAsync();

            var studentCourseEnrollments = new List<CanvasEnrollment>();

            studentCanvasUsers.ForEach(srr =>
            {
                var enrollment = srr.enrollments.First();
                enrollment.user = srr;
                studentCourseEnrollments.Add(enrollment);
            });

            var canvasEnrollmentsReturned = new List<CanvasEnrollment>();
            canvasEnrollmentsReturned.AddRange(studentCourseEnrollments);
            canvasEnrollmentsReturned.AddRange(faCanvasEnrollments);


            ecatCourse = CanvasBusinessLogic.ReconcileCourseMembers(ecatCourse, canvasEnrollmentsReturned, Faculty.PersonId);

            var facAddCount = ecatCourse.FacultyToReconcile.Count(reconcile => reconcile.NewEnrollment);
            var stuAddCount = ecatCourse.StudentsToReconcile.Count(reconcile => reconcile.NewEnrollment);
            var totalAdd = facAddCount + stuAddCount;

            var facDeleteCount = ecatCourse.FacultyToReconcile.Count(reconcile => reconcile.RemoveEnrollment);
            var stuDeleteCount = ecatCourse.StudentsToReconcile.Count(reconcile => reconcile.RemoveEnrollment);
            var totalDelete = facDeleteCount + stuDeleteCount;

            if (totalAdd > 0)
            {
                courseMemReconResult = await AddCanvasCourseEnrollments(ecatCourse, courseMemReconResult);
            }

            if (totalDelete > 0)
            {
                courseMemReconResult.RemovedIds = await RemoveCanvasCourseEnrollments(ecatCourse, courseMemReconResult);
                courseMemReconResult.NumRemoved = totalDelete;
            }

            return courseMemReconResult;

        }

        private async Task<List<int>> RemoveCanvasCourseEnrollments(CourseReconcile ecatCourse, MemReconResult reconResult)
        {

            var memRemovedIds = new List<int>();

            var stusToRemove = ecatCourse.StudentsToReconcile.Where(stuToReconcile => stuToReconcile.RemoveEnrollment)
                .Select(stuToReconcile => stuToReconcile).ToList();

            var facultyToRemove = ecatCourse.FacultyToReconcile.Where(facToReconcile => facToReconcile.RemoveEnrollment)
                .Select(facToReconcile => facToReconcile).ToList();

            stusToRemove.ForEach(stuToRemove =>
            {
                var studentInCourse = new StudentInCourse
                {
                    StudentPersonId = stuToRemove.PersonId,
                    CourseId = ecatCourse.Course.Id
                };

                if (stuToRemove.FlagDeleted)
                {
                    studentInCourse.DeletedDate = DateTime.Now;
                    studentInCourse.DeletedById = Faculty.PersonId;
                    studentInCourse.IsDeleted = true;
                    ctxManager.Context.Entry(studentInCourse).State = EntityState.Modified;
                    return;
                }

                //TODO: Clean up sp data if groups member is in are not published

                ctxManager.Context.Entry(studentInCourse).State = EntityState.Deleted;
                memRemovedIds.Add(stuToRemove.PersonId);

            });

            facultyToRemove.ForEach(facToRemove =>
            {
                var facInCourse = new FacultyInCourse
                {
                    FacultyPersonId = facToRemove.PersonId,
                    CourseId = ecatCourse.Course.Id
                };

                if (facToRemove.FlagDeleted)
                {
                    facInCourse.DeletedDate = DateTime.Now;
                    facInCourse.DeletedById = Faculty.PersonId;
                    facInCourse.IsDeleted = true;
                    ctxManager.Context.Entry(facInCourse).State = EntityState.Modified;
                    return;
                }

                //TODO: Clean up sp data if groups member is in are not published

                ctxManager.Context.Entry(facInCourse).State = EntityState.Deleted;
                memRemovedIds.Add(facToRemove.PersonId);

            });

            await ctxManager.Context.SaveChangesAsync();

            return memRemovedIds;
        }

        private async Task<MemReconResult> AddCanvasCourseEnrollments(CourseReconcile ecatCourse, MemReconResult reconResult)
        {

            var memsToAdd = ecatCourse.StudentsToReconcile.Where(stuToReconcile => stuToReconcile.NewEnrollment)
                .Select(stuToReconcile => stuToReconcile).ToList();

            memsToAdd.AddRange(ecatCourse.FacultyToReconcile.Where(facToReconcile => facToReconcile.NewEnrollment)
                .Select(facToReconcile => facToReconcile).ToList());

            var memsToAddIds = memsToAdd.Select(memToAdd => memToAdd.BbUserId).ToList();

            var usersWithEcatAccount = await ctxManager.Context.People
                .Where(p => memsToAddIds.Contains(p.BbUserId))
                .Include(p =>p.Student)
                .Include(p => p.Faculty)
                .ToListAsync();

            var accountsNeedToCreate = memsToAdd
                .Where(memToAdd => !usersWithEcatAccount.Select(userWithEcatAccount => userWithEcatAccount.BbUserId).Contains(memToAdd.BbUserId)).ToList();

            if (accountsNeedToCreate.Any())
            {
                accountsNeedToCreate.ForEach(accountNeedToCreate =>
                {
                    ctxManager.Context.People.Add(accountNeedToCreate.Person);
                });

                reconResult.NumOfAccountCreated = await ctxManager.Context.SaveChangesAsync();

            }

            //Create list of Users that need profiles
            var usersWithoutProfile = new List<Person>();

            if (usersWithEcatAccount.Any())
            {
                usersWithoutProfile = usersWithEcatAccount.Where(userWithoutEatAccount =>
                    userWithoutEatAccount.Faculty == null && userWithoutEatAccount.Student == null).ToList();
            }

            usersWithoutProfile.AddRange(accountsNeedToCreate.Select(account => account.Person).ToList());

            //TODO: What happens if a faculty is moved to a student in the same course? RIght now it errors. Possible workaround remove instructor from course, sync. Then add back as student and sync

            if (usersWithoutProfile.Any())
            {
                usersWithoutProfile.ForEach(userWithoutProfile =>
                {
      
                    switch (userWithoutProfile.MpInstituteRole)
                    {
                        case MpInstituteRoleId.Faculty:
                            userWithoutProfile.Faculty = new ProfileFaculty
                            {
                                PersonId = userWithoutProfile.PersonId,
                                AcademyId = reconResult.AcademyId,
                                HomeStation = StaticAcademy.AcadLookupById[reconResult.AcademyId].Base.ToString(),
                                IsReportViewer = false,
                                IsCourseAdmin = false
                            };
                            ctxManager.Context.Faculty.Add(userWithoutProfile.Faculty);
                            break;
                        case MpInstituteRoleId.Student:
                            userWithoutProfile.Student = new ProfileStudent
                            {
                                PersonId = userWithoutProfile.PersonId
                            };
                            ctxManager.Context.Students.Add(userWithoutProfile.Student);
                            break;
                        default:
                            userWithoutProfile.Student = new ProfileStudent
                            {
                                PersonId = userWithoutProfile.PersonId
                            };
                            ctxManager.Context.Students.Add(userWithoutProfile.Student);
                            break;
                    }            

                });

                await ctxManager.Context.SaveChangesAsync();

            }

            /////////////////
                   
            //Add Users to StudentInCourse

            reconResult.Students = new List<StudentInCourse>();
            reconResult.Faculty = new List<FacultyInCourse>();

            memsToAdd.ForEach(memToAdd =>
            {

                var personId = memToAdd.Person?.PersonId ?? 0;

                if (personId == 0)
                {
                    var person = usersWithEcatAccount
                        .FirstOrDefault(userWithEcatAccount => userWithEcatAccount.BbUserId == memToAdd.BbUserId);
                    memToAdd.Person = person;
                    personId = person.PersonId;
                }
                
                switch (memToAdd.Person.MpInstituteRole)
                {
                    case MpInstituteRoleId.Faculty:
                        var facultyInCourse = new FacultyInCourse
                        {
                            //If User has ECAT account use personId from reconcile if not use personId from newly created person
                            FacultyPersonId = personId,
                            CourseId = reconResult.CourseId
                        };

                        if (memToAdd.UnFlagDeleted)
                        {
                            facultyInCourse.IsDeleted = false;
                            facultyInCourse.DeletedDate = null;
                            facultyInCourse.DeletedById = null;
                            ctxManager.Context.FacultyInCourses.Attach(facultyInCourse);
                            ctxManager.Context.Entry(facultyInCourse).State = System.Data.Entity.EntityState.Modified;
                            break;
                        }
                      
                        facultyInCourse.ReconResultId = reconResult.Id;
                        //facultyInCourse.BbCourseMemId = memToAdd.CourseMemId;                     
                        reconResult.Faculty.Add(facultyInCourse);
                        break;
                    case MpInstituteRoleId.Student:
                        var studentInCourse = new StudentInCourse
                        {
                            StudentPersonId = personId,
                            CourseId = reconResult.CourseId,
                            
                        };

                        if (memToAdd.UnFlagDeleted)
                        {
                            studentInCourse.IsDeleted = false;
                            studentInCourse.DeletedDate = null;
                            studentInCourse.DeletedById = null;                      
                            ctxManager.Context.StudentInCourses.Attach(studentInCourse);
                            ctxManager.Context.Entry(studentInCourse).State = System.Data.Entity.EntityState.Modified;
                            break;
                        }

                        studentInCourse.ReconResultId = reconResult.Id;
                        //studentInCourse.BbCourseMemId = memToAdd.CourseMemId;
                        reconResult.Students.Add(studentInCourse);
                        break;
                    default:
                        var studentInCourseDefault = new StudentInCourse
                        {
                            StudentPersonId = personId,
                            CourseId = reconResult.CourseId,

                        };

                        if (memToAdd.UnFlagDeleted)
                        {
                            studentInCourseDefault.IsDeleted = false;
                            studentInCourseDefault.DeletedDate = null;
                            studentInCourseDefault.DeletedById = null;
                            ctxManager.Context.StudentInCourses.Attach(studentInCourseDefault);
                            ctxManager.Context.Entry(studentInCourseDefault).State = System.Data.Entity.EntityState.Modified;
                            break;
                        }

                        studentInCourseDefault.ReconResultId = reconResult.Id;
                        //studentInCourseDefault.BbCourseMemId = memToAdd.CourseMemId;
                        reconResult.Students.Add(studentInCourseDefault);
                        break;
                }
            });

            if (reconResult.Students.Any())
            {
                ctxManager.Context.StudentInCourses.AddRange(reconResult.Students);
            }

            
            if (reconResult.Faculty.Any()){
                ctxManager.Context.FacultyInCourses.AddRange(reconResult.Faculty);
            }

            if (ctxManager.Context.ChangeTracker.HasChanges())
            {
                reconResult.NumAdded += await ctxManager.Context.SaveChangesAsync();
            }

            return reconResult;
        }

        private async Task<GroupReconResult> SyncCanvasSections(int crseId, List<CanvasSection> canvasSectionsReturned)
        {
            var course = await ctxManager.Context.Courses.Where(c => c.Id == crseId)
                .Include(c => c.Students)
                .Include(c => c.WorkGroups)
                .SingleAsync();

            var academy = StaticAcademy.AcadLookupById[course.AcademyId];
            var workGroupModel = await ctxManager.Context.WgModels
                .Where(wgm => wgm.IsActive && wgm.MpEdLevel == academy.MpEdLevel && wgm.MpWgCategory == MpGroupCategory.Wg1)
                .SingleAsync();

            var workGroups = course.WorkGroups;

            var reconResult = new GroupReconResult
            {
                Id = Guid.NewGuid(),
                AcademyId = Faculty.AcademyId,
                Groups = new List<WorkGroup>()
            };



            if (canvasSectionsReturned == null) return reconResult;

            var sectionsToAdd = CanvasBusinessLogic.ReconcileWorkGroups(canvasSectionsReturned, workGroups, crseId, workGroupModel, Faculty.PersonId, reconResult.Id);

            if (sectionsToAdd.Count == 0) return reconResult;

            sectionsToAdd.ForEach(sta =>
            {
                ctxManager.Context.WorkGroups.Add(sta);
                reconResult.Groups.Add(sta);
            });

            await ctxManager.Context.SaveChangesAsync();

            return reconResult;

        }

        //Old Blackboard API Calls

        //public async Task<MemReconResult> ReconcileCourseMembers(int courseId)
        //{
        //    //await GetProfile();

        //    var ecatCourse = await ctxManager.Context.Courses
        //        .Where(crse => crse.Id == courseId)
        //        .Select(crse => new CourseReconcile
        //        {
        //            Course = crse,
        //            FacultyToReconcile = crse.Faculty.Select(fac => new UserReconcile
        //            {
        //                PersonId = fac.FacultyPersonId,
        //                BbUserId = fac.FacultyProfile.Person.BbUserId,
        //                CanDelete = !fac.FacSpComments.Any() &&
        //                            !fac.FacSpResponses.Any() &&
        //                            !fac.FacStratResponse.Any()
        //            }).ToList(),
        //            StudentsToReconcile = crse.Students.Select(sic => new UserReconcile
        //            {
        //                PersonId = sic.StudentPersonId,
        //                BbUserId = sic.Student.Person.BbUserId,
        //                CanDelete = !sic.WorkGroupEnrollments.Any()
        //            }).ToList()
        //        }).SingleOrDefaultAsync();

        //    //This is for debugging
        //    Contract.Assert(ecatCourse != null);

        //    var reconResult = new MemReconResult
        //    {
        //        Id = Guid.NewGuid(),
        //        CourseId = courseId,
        //        AcademyId = Faculty?.AcademyId
        //    };

        //    var autoRetryCm = new Retrier<CourseMembershipVO[]>();

        //    var courseMemFilter = new MembershipFilter
        //    {
        //        filterTypeSpecified = true,
        //        filterType = (int)CrseMembershipFilterType.LoadByCourseId,
        //    };

        //    var bbCourseMems =
        //        await autoRetryCm.Try(() => _bbWs.BbCourseMembership(ecatCourse.Course.BbCourseId, courseMemFilter), 3);

        //    var existingCrseUserIds = ecatCourse.FacultyToReconcile
        //        .Select(fac => fac.BbUserId).ToList();

        //    existingCrseUserIds.AddRange(ecatCourse.StudentsToReconcile.Select(sic => sic.BbUserId));

        //    var newMembers = bbCourseMems
        //        .Where(cm => !existingCrseUserIds.Contains(cm.userId))
        //        .Where(cm => cm.available == true)
        //        .ToList();

        //    if (newMembers.Any())
        //    {
        //        //var queryCr = await autoRetryCr.Try(client.getCourseRolesAsync(bbCourseMems.Select(bbcm => bbcm.roleId).ToArray()), 3);

        //        //var bbCourseRoles = queryCr.@return.ToList();
        //        reconResult = await AddNewUsers(newMembers, reconResult);
        //        reconResult.NumAdded = newMembers.Count();
        //    }

        //    var usersBbIdsToRemove = existingCrseUserIds
        //        .Where(ecu => !bbCourseMems.Select(cm => cm.userId).Contains(ecu)).ToList();

        //    if (usersBbIdsToRemove.Any())
        //    {
        //        reconResult.RemovedIds = await RemoveOrFlagUsers(ecatCourse, usersBbIdsToRemove);
        //        reconResult.NumRemoved = reconResult.RemovedIds.Count();
        //    }

        //    return reconResult;
        //}

        //private async Task<MemReconResult> AddNewUsers(IEnumerable<CourseMembershipVO> bbCmsVo,
        //    MemReconResult reconResult)
        //{
        //    var bbCms = bbCmsVo.ToList();
        //    var bbCmUserIds = bbCms.Select(bbcm => bbcm.userId).ToList();

        //    var usersWithAccount = await ctxManager.Context.People
        //        .Where(p => bbCmUserIds.Contains(p.BbUserId))
        //        .Select(p => new
        //        {
        //            p.BbUserId,
        //            p.PersonId,
        //            p.MpInstituteRole
        //        })
        //        .ToListAsync();

        //    var accountsNeedToCreate =
        //        bbCms.Where(cm => !usersWithAccount.Select(uwa => uwa.BbUserId).Contains(cm.userId)).ToList();

        //    if (accountsNeedToCreate.Any())
        //    {
        //        var userFilter = new UserFilter
        //        {
        //            filterTypeSpecified = true,
        //            filterType = (int)UserFilterType.UserByIdWithAvailability,
        //            available = true,
        //            availableSpecified = true,
        //            id = accountsNeedToCreate.Select(bbAccount => bbAccount.userId).ToArray()
        //        };

        //        var autoRetryUsers = new Retrier<UserVO[]>();
        //        var bbUsers = await autoRetryUsers.Try(() => _bbWs.BbCourseUsers(userFilter), 3);
        //        var courseMems = bbCms;

        //        if (bbUsers != null)
        //        {

        //            var users = bbUsers.Select(bbu =>
        //            {
        //                var cm = courseMems.First(bbcm => bbcm.userId == bbu.id);
        //                return new Person
        //                {
        //                    MpInstituteRole = MpRoleTransform.BbWsRoleToEcat(cm.roleId),
        //                    BbUserId = bbu.id,
        //                    BbUserName = bbu.name,
        //                    Email = $"{cm.id}-{bbu.extendedInfo.emailAddress}",
        //                    IsActive = true,
        //                    LastName = bbu.extendedInfo.familyName,
        //                    FirstName = bbu.extendedInfo.givenName,
        //                    MpGender = MpGender.Unk,
        //                    MpAffiliation = MpAffiliation.Unk,
        //                    MpComponent = MpComponent.Unk,
        //                    RegistrationComplete = false,
        //                    MpPaygrade = MpPaygrade.Unk,
        //                    ModifiedById = Faculty.PersonId,
        //                    ModifiedDate = DateTime.Now
        //                };
        //            }).ToList();

        //            foreach (var user in users)
        //            {
        //                ctxManager.Context.People.Add(user);

        //            }

        //            reconResult.NumOfAccountCreated = await ctxManager.Context.SaveChangesAsync();

        //            foreach (var user in users)
        //            {
        //                usersWithAccount.Add(new
        //                {
        //                    user.BbUserId,
        //                    user.PersonId,
        //                    user.MpInstituteRole
        //                });

        //                switch (user.MpInstituteRole)
        //                {
        //                    case MpInstituteRoleId.Faculty:
        //                        user.Faculty = new ProfileFaculty
        //                        {
        //                            PersonId = user.PersonId,
        //                            AcademyId = reconResult.AcademyId,
        //                            HomeStation = StaticAcademy.AcadLookupById[reconResult.AcademyId].Base.ToString(),
        //                            IsReportViewer = false,
        //                            IsCourseAdmin = false
        //                        };
        //                        break;
        //                    case MpInstituteRoleId.Student:
        //                        user.Student = new ProfileStudent
        //                        {
        //                            PersonId = user.PersonId
        //                        };
        //                        break;
        //                    default:
        //                        user.Student = new ProfileStudent
        //                        {
        //                            PersonId = user.PersonId
        //                        };
        //                        break;
        //                }
        //            }

        //            await ctxManager.Context.SaveChangesAsync();
        //        }
        //    }

        //    reconResult.Students = usersWithAccount
        //        .Where(ecm =>
        //        {
        //            var bbCM = bbCmsVo.First(cm => cm.userId == ecm.BbUserId);
        //            return MpRoleTransform.BbWsRoleToEcat(bbCM.roleId) != MpInstituteRoleId.Faculty;
        //        })
        //        .Select(ecm => new StudentInCourse
        //        {
        //            StudentPersonId = ecm.PersonId,
        //            CourseId = reconResult.CourseId,
        //            ReconResultId = reconResult.Id,
        //            BbCourseMemId = bbCms.First(bbcm => bbcm.userId == ecm.BbUserId).id
        //        }).ToList();

        //    reconResult.Faculty = usersWithAccount
        //        .Where(ecm =>
        //        {
        //            var bbCM = bbCmsVo.First(cm => cm.userId == ecm.BbUserId);
        //            return MpRoleTransform.BbWsRoleToEcat(bbCM.roleId) == MpInstituteRoleId.Faculty;
        //        })
        //        .Select(ecm => new FacultyInCourse
        //        {
        //            FacultyPersonId = ecm.PersonId,
        //            CourseId = reconResult.CourseId,
        //            ReconResultId = reconResult.Id,
        //            BbCourseMemId = bbCms.First(bbcm => bbcm.userId == ecm.BbUserId).id
        //        }).ToList();

        //    var neededFacultyProfiles = usersWithAccount.Where(ecm =>
        //    {
        //        var bbCM = bbCmsVo.First(cm => cm.userId == ecm.BbUserId);
        //        return MpRoleTransform.BbWsRoleToEcat(bbCM.roleId) == MpInstituteRoleId.Faculty;
        //    }).Select(ecm => ecm.PersonId);

        //    var existingFacultyProfiles = ctxManager.Context.Faculty
        //        .Where(fac => neededFacultyProfiles.Contains(fac.PersonId))
        //        .Select(fac => fac.PersonId);

        //    var newFacultyProfiles = neededFacultyProfiles
        //        .Where(id => !existingFacultyProfiles.Contains(id))
        //        .Select(id => new ProfileFaculty
        //        {
        //            PersonId = id,
        //            AcademyId = reconResult.AcademyId,
        //            HomeStation = StaticAcademy.AcadLookupById[reconResult.AcademyId].Base.ToString(),
        //            IsReportViewer = false,
        //            IsCourseAdmin = false
        //        });

        //    var neededStudentProfiles = usersWithAccount.Where(ecm =>
        //    {
        //        var bbCM = bbCmsVo.First(cm => cm.userId == ecm.BbUserId);
        //        return MpRoleTransform.BbWsRoleToEcat(bbCM.roleId) != MpInstituteRoleId.Faculty;
        //    }).Select(ecm => ecm.PersonId);

        //    var existingStudentProfiles = ctxManager.Context.Students
        //        .Where(stud => neededStudentProfiles.Contains(stud.PersonId))
        //        .Select(stud => stud.PersonId);

        //    var newStudentProfiles = neededStudentProfiles
        //        .Where(id => !existingStudentProfiles.Contains(id))
        //        .Select(id => new ProfileStudent
        //        {
        //            PersonId = id,
        //        });

        //    if (newFacultyProfiles.Any()) ctxManager.Context.Faculty.AddRange(newFacultyProfiles);
        //    if (newStudentProfiles.Any()) ctxManager.Context.Students.AddRange(newStudentProfiles);

        //    if (reconResult.Students.Any()) ctxManager.Context.StudentInCourses.AddRange(reconResult.Students);

        //    if (reconResult.Faculty.Any()) ctxManager.Context.FacultyInCourses.AddRange(reconResult.Faculty);

        //    reconResult.NumAdded = reconResult.Faculty.Any() || reconResult.Students.Any()
        //        ? await ctxManager.Context.SaveChangesAsync()
        //        : 0;

        //    return reconResult;
        //}

        //private async Task<List<int>> RemoveOrFlagUsers(CourseReconcile courseToReconcile,
        //    IEnumerable<string> bbIdsToRemove)
        //{
        //    var idsRemoved = new List<int>();

        //    foreach (var studReconcile in courseToReconcile.StudentsToReconcile.Where(str =>
        //        bbIdsToRemove.Contains(str.BbUserId)))
        //    {
        //        idsRemoved.Add(studReconcile.PersonId);
        //        var sic = new StudentInCourse
        //        {
        //            StudentPersonId = studReconcile.PersonId,
        //            CourseId = courseToReconcile.Course.Id
        //        };

        //        if (studReconcile.CanDelete)
        //        {
        //            ctxManager.Context.Entry(sic).State = System.Data.Entity.EntityState.Deleted;
        //        }
        //        else
        //        {
        //            sic.DeletedDate = DateTime.Now;
        //            sic.DeletedById = Faculty.PersonId;
        //            sic.IsDeleted = true;
        //            ctxManager.Context.Entry(sic).State = System.Data.Entity.EntityState.Modified;
        //        }
        //    }

        //    foreach (var facReconcile in courseToReconcile.FacultyToReconcile.Where(str =>
        //        bbIdsToRemove.Contains(str.BbUserId)))
        //    {
        //        idsRemoved.Add(facReconcile.PersonId);
        //        var fic = new FacultyInCourse
        //        {
        //            FacultyPersonId = facReconcile.PersonId,
        //            CourseId = courseToReconcile.Course.Id
        //        };

        //        if (facReconcile.CanDelete)
        //        {
        //            ctxManager.Context.Entry(fic).State = System.Data.Entity.EntityState.Deleted;
        //        }
        //        else
        //        {
        //            fic.DeletedDate = DateTime.Now;
        //            fic.DeletedById = Faculty.PersonId;
        //            fic.IsDeleted = true;
        //            ctxManager.Context.Entry(fic).State = System.Data.Entity.EntityState.Modified;
        //        }
        //    }

        //    await ctxManager.Context.SaveChangesAsync();

        //    return idsRemoved;
        //}

        //public async Task<CourseReconResult> ReconcileCourses()
        //{
        //    //await GetProfile();

        //    var courseFilter = new CourseFilter
        //    {
        //        filterTypeSpecified = true,
        //        filterType = (int)CourseFilterType.LoadByCatId
        //    };

        //    if (Faculty != null)
        //    {
        //        var academy = StaticAcademy.AcadLookupById[Faculty.AcademyId];
        //        courseFilter.categoryIds = new[] { academy.BbCategoryId };
        //    }
        //    else
        //    {
        //        var ids = StaticAcademy.AcadLookupById.Select(acad => acad.Value.BbCategoryId).ToArray();
        //        courseFilter.categoryIds = ids;
        //    }

        //    var autoRetry = new Retrier<CourseVO[]>();
        //    var bbCoursesResult = await autoRetry.Try(() => _bbWs.BbCourses(courseFilter), 3);

        //    if (bbCoursesResult == null) throw new InvalidDataException("No Bb Responses received");

        //    var queryKnownCourses = ctxManager.Context.Courses.AsQueryable();

        //    queryKnownCourses = Faculty == null
        //        ? queryKnownCourses
        //        : queryKnownCourses.Where(crse => crse.AcademyId == Faculty.AcademyId);

        //    var knownCoursesIds = queryKnownCourses.Select(crse => crse.BbCourseId).ToList();

        //    var reconResult = new CourseReconResult
        //    {
        //        Id = Guid.NewGuid(),
        //        AcademyId = Faculty?.AcademyId,
        //        Courses = new List<Course>()
        //    };

        //    foreach (var nc in bbCoursesResult
        //        .Where(bbc => !knownCoursesIds.Contains(bbc.id))
        //        .Select(bbc => new Course
        //        {
        //            BbCourseId = bbc.id,
        //            AcademyId = Faculty.AcademyId,
        //            Name = bbc.name,
        //            StartDate = DateTime.Now,
        //            GradDate = DateTime.Now.AddDays(25)
        //        }))
        //    {
        //        reconResult.NumAdded += 1;
        //        reconResult.Courses.Add(nc);
        //        ctxManager.Context.Courses.Add(nc);
        //    }

        //    await ctxManager.Context.SaveChangesAsync();

        //    foreach (var course in reconResult.Courses)
        //    {
        //        course.ReconResultId = reconResult.Id;
        //    }

        //    return reconResult;
        //}

        //public async Task<List<CategoryVO>> GetBbCategories()
        //{
        //    var filter = new CategoryFilter
        //    {
        //        filterType = (int)CategoryFilterTpe.GetAllCourseCategory,
        //        filterTypeSpecified = true
        //    };
        //    var autoRetry = new Retrier<CategoryVO[]>();
        //    var categories = await autoRetry.Try(() => _bbWs.BbCourseCategories(filter), 3);

        //    return categories.ToList();
        //}
    }

}

