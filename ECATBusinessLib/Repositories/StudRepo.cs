using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
//using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Data.Models.Common;
//using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Data.Models.Student;
//using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Data.Models.School;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
//using Ecat.Shared.Core.Utility;
//using Ecat.Shared.DbMgr.Context;
using Ecat.Data.Contexts;
using Ecat.Business.Repositories.Interface;
using Ecat.Business.Guards;
using Ecat.Data.Static;
using Newtonsoft.Json.Linq;

namespace Ecat.Business.Repositories
{

    public class StudRepo : IStudRepo
    {
        private readonly EFContextProvider<EcatContext> _efCtx;

        public Person StudentPerson { get; set; }

        public StudRepo(EFContextProvider<EcatContext> efCtx)
        {
            _efCtx = efCtx;
        }

        public SaveResult ClientSave(JObject saveBundle)
        {
            var guardian = new StudentGuardian(_efCtx, StudentPerson);
            _efCtx.BeforeSaveEntitiesDelegate += guardian.BeforeSaveEntities;
            return _efCtx.SaveChanges(saveBundle);
        }

        async Task<List<Course>> IStudRepo.GetCourses(int? crseId)
        {
            var query = crseId != null ? _efCtx.Context.Courses.Where(c => c.Id == crseId) : _efCtx.Context.Courses;

            var studCourseInit = await (from crse in query
                where crse.Students.Any(sic => sic.StudentPersonId == StudentPerson.PersonId && !sic.IsDeleted)
                select new
                {
                    crse,
                    workGroups =
                        crse.WorkGroups.Where(wg => wg.GroupMembers.Any(gm => gm.StudentId == StudentPerson.PersonId && !gm.IsDeleted)),
                    StudentCoures = crse.Students
                        .Where(sic => sic.StudentPersonId == StudentPerson.PersonId),
                }).ToListAsync();


            var requestedCourses = studCourseInit.Select(sic => sic.crse).ToList();

            if (requestedCourses == null || !requestedCourses.Any())
            {
                return null;
            }

            var activeCourse = requestedCourses.OrderByDescending(crse => crse.StartDate).First();
            if (activeCourse.WorkGroups == null) return requestedCourses;
            var activeGroup = activeCourse.WorkGroups.OrderByDescending(wg => wg.MpCategory).FirstOrDefault();
            if (crseId != null) requestedCourses = requestedCourses.Where(crse => crse.Id == crseId).ToList();

            if (activeGroup == null) return requestedCourses;

            activeGroup = await GetWorkGroup(activeGroup.WorkGroupId, true);
            activeCourse.WorkGroups.Add(activeGroup);
            return requestedCourses;
        }

        public async Task<WorkGroup> GetWorkGroup(int wgId, bool addInstrument)
        {
            var workGroup = await (from wg in _efCtx.Context.WorkGroups
                where wg.WorkGroupId == wgId &&
                      wg.GroupMembers.Any(gm => gm.StudentId == StudentPerson.PersonId && !gm.IsDeleted)
                let prundedGm = wg.GroupMembers.Where(gm => !gm.IsDeleted)
                let myPrunded = prundedGm.FirstOrDefault(gm => gm.StudentId == StudentPerson.PersonId)
                select new
                {
                    wg,
                    wg.AssignedSpInstr,
                    wg.AssignedSpInstr.InventoryCollection,
                    PrunedGroupMembers = prundedGm,
                    myPrunded.AssessorSpResponses,
                    myPrunded.AssessorStratResponse,
                    myPrunded.AuthorOfComments,
                    Flags = myPrunded.AuthorOfComments.Select(aoc => aoc.Flag),
                    Profiles = prundedGm.Select(gm => gm.StudentProfile).Distinct(),
                    Persons = prundedGm.Select(gm => gm.StudentProfile.Person).Distinct()
                }).SingleOrDefaultAsync();

            var requestedWorkGroup = workGroup.wg;
            if (addInstrument) return requestedWorkGroup;

            requestedWorkGroup.AssignedSpInstr.InventoryCollection = null;
            requestedWorkGroup.AssignedSpInstr = null;

            return requestedWorkGroup;
        }

        async Task<SpResult> IStudRepo.GetWrkGrpResult(int wgId, bool addInstrument)
        {
            var myResult = await (from result in _efCtx.Context.SpResults
                where result.WorkGroup.MpSpStatus == MpSpStatus.Published &&
                      result.StudentId == StudentPerson.PersonId &&
                      result.WorkGroupId == wgId
                let prundedGm = result.WorkGroup.GroupMembers.Where(gm => !gm.IsDeleted)
                let myPrunded = prundedGm.FirstOrDefault(gm => gm.StudentId == StudentPerson.PersonId)
                select new
                {
                    result,
                    result.CourseId,
                    result.WorkGroup,
                    result.WorkGroup.AssignedSpInstr,
                    result.WorkGroup.AssignedSpInstr.InventoryCollection,
                    prundedGm,
                    Profiles = prundedGm.Select(gm => gm.StudentProfile).Distinct(),
                    Persons = prundedGm.Select(gm => gm.StudentProfile.Person).Distinct(),
                    SpAssessorResponses = myPrunded.AssessorSpResponses,
                    SpAssesseeResponses = myPrunded.AssesseeSpResponses,
                    SpStratResponse = myPrunded.AssessorStratResponse,
                    AuthorComments = myPrunded.AuthorOfComments,
                    RecipientComments = myPrunded.RecipientOfComments,
                    AuthorFlags = myPrunded.AuthorOfComments.Select(aoc => aoc.Flag),
                    RecipientFlags = myPrunded.RecipientOfComments.Select(aoc => aoc.Flag)
                }).SingleOrDefaultAsync();

            if (myResult == null) return null;

            var requestedResult = myResult.result;

            requestedResult.WorkGroup.SpResponses =
                requestedResult.WorkGroup.SpResponses.Where(
                    response => response.AssessorPersonId == StudentPerson.PersonId).ToList();

            requestedResult.WorkGroup.SpStratResponses =
                requestedResult.WorkGroup.SpStratResponses.Where(
                    response => response.AssessorPersonId == StudentPerson.PersonId).ToList();


            if (addInstrument)
            {
                requestedResult.AssignedInstrument.InventoryCollection = null;
                requestedResult.AssignedInstrument = null;
            }

            var facResultData = await GetFacSpResult(StudentPerson.PersonId, wgId);

            requestedResult.SanitizedResponses = myResult.SpAssesseeResponses.Select(ar => new SanitizedSpResponse
            {
                //StudentId = StudentPerson.PersonId,
                Id = Guid.NewGuid(),
                AssesseeId = StudentPerson.PersonId,
                CourseId = myResult.WorkGroup.CourseId,
                WorkGroupId = myResult.WorkGroup.WorkGroupId,
                IsSelfResponse = ar.AssessorPersonId == StudentPerson.PersonId,
                MpItemResponse = ar.MpItemResponse,
                ItemModelScore = ar.ItemModelScore,
                InventoryItemId = ar.InventoryItemId,
                Result = requestedResult
            }).ToList();

            var apprComments =
                myResult.RecipientComments.Where(comment => comment.Flag.MpFaculty == MpCommentFlag.Appr).ToList();

            requestedResult.SanitizedComments = apprComments.Select(rc => new SanitizedSpComment
            {
                RecipientId = rc.RecipientPersonId,
                Id = Guid.NewGuid(),
                CourseId = rc.CourseId,
                WorkGroupId = rc.WorkGroupId,
                AuthorName =
                    (rc.RequestAnonymity)
                        ? "Anonymous"
                        : $"{rc.Author.StudentProfile.Person.FirstName} {rc.Author.StudentProfile.Person.LastName}",
                CommentText = rc.CommentText,
                Flag =
                    myResult.RecipientFlags.Single(
                        flag =>
                            flag.AuthorPersonId == rc.AuthorPersonId && flag.RecipientPersonId == rc.RecipientPersonId)
            }).ToList();

            foreach (var flag in requestedResult.SanitizedComments.Select(sc => sc.Flag))
            {
                flag.AuthorPersonId = flag.AuthorPersonId == StudentPerson.PersonId
                    ? flag.AuthorPersonId
                    : flag.AuthorPersonId*13;
            }

            requestedResult.FacultyResponses = facResultData.FacResponses.ToList();

            if (facResultData.FacSpComment != null)
            {
                requestedResult.SanitizedComments.Add(new SanitizedSpComment
                {
                    Id = Guid.NewGuid(),
                    RecipientId = facResultData.FacSpComment.RecipientPersonId,
                    CourseId = myResult.CourseId,
                    WorkGroupId = myResult.WorkGroup.WorkGroupId,
                    AuthorName = "Instructor",
                    CommentText = facResultData.FacSpComment.CommentText,
                    FacFlag = facResultData.FacSpCommentFlag
                });
            }

            return requestedResult;
        }

        public string GetMetadata
        {
            get
            {
                //TODO: ECAT2
                var metadataCtx = new EFContextProvider<EcatContext>();
                return metadataCtx.Metadata();
            }
        }

        private static async Task<FacResultForStudent> GetFacSpResult(int studId, int wgId)
        {
            using (var mainCtx = new EcatContext())
            {
                var result = await mainCtx.WorkGroups
                    .Where(wg => wg.WorkGroupId == wgId)
                    .Select(wg => new FacResultForStudent
                    {
                        FacSpCommentFlag = wg.FacSpComments
                            .FirstOrDefault(comment => comment.RecipientPersonId == studId).Flag,
                        FacSpComment = wg.FacSpComments.FirstOrDefault(comment => comment.RecipientPersonId == studId),
                        FacResponses = wg.FacSpResponses
                            .Where(response => !response.IsDeleted &&
                                               response.AssesseePersonId == studId).ToList()
                    }).SingleOrDefaultAsync();
                return result;
            }
        }
    }
}
