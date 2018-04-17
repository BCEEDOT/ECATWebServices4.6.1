using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.Core.Utility;
using Ecat.Shared.DbMgr.BbWs.BbCourse;
using Ecat.Shared.DbMgr.Context;
using Newtonsoft.Json.Linq;

namespace Ecat.FacMod.Core
{
    public class FacRepo : IFacRepo
    {
        private readonly FacCtx _ctx;
        private readonly EFContextProvider<FacCtx> _efCtx;

        public FacRepo(FacCtx ctx, EFContextProvider<FacCtx> efCtx)
        {
            _ctx = ctx;
            _efCtx = efCtx;
        }

        void IFacRepo.AddCourseWorkgroups(Course course)
        {
            _ctx.Entry(course).Collection(wg => wg.WorkGroups).Load();
        }

        IQueryable<CommentCount> IFacRepo.AuthorCommentCounts(List<int> authorIds, int workGroupId)
        {
            return _ctx.StudentInGroups
                .Where(sig => authorIds.Contains(sig.StudentId) && sig.WorkGroupId == workGroupId)
                .Select(sig => new CommentCount
                {
                    AuthorId = sig.StudentId,
                    NumOfComments = sig.AuthorOfComments.Count()
                });
        }

        SaveResult IFacRepo.ClientSaveChanges(JObject saveBundle, Person loggedInUser)
        {
            var guardian = new FacultyGuardian(_efCtx, loggedInUser);
            _efCtx.BeforeSaveEntitiesDelegate += guardian.BeforeSaveEntities;

            return _efCtx.SaveChanges(saveBundle);
        }

        string IFacRepo.Metadata => _efCtx.Metadata();

        IQueryable<Course> IFacRepo.GetCourses => _ctx.Courses;

        //async Task<List<CourseVO>> IFacRepo.BbCourses(string academyCatId)
        //{
        //    var bbWs = new BbWsCnet();

        //    var academyCatArray = new[] {academyCatId};

        //    var courseClient = await bbWs.GetCourseClient();

        //    var courseFilter = new CourseFilter
        //    {
        //        available = 1, //0:false/1:true/2:all 
        //        availableSpecified = true,
        //        filterType = (int) CourseFilterType.LoadByCatId,
        //        filterTypeSpecified = true,
        //        categoryIds = academyCatArray
        //    };

        //    var result = await courseClient.getCourseAsync(courseFilter);

        //    return result.@return.ToList();
        //}

        async Task<Person> IFacRepo.LoadFacultyProfile(Person faculty)
        {
            await _ctx.Entry(faculty).Reference(p => p.Faculty).LoadAsync();
            return faculty;
        }

        IQueryable<WorkGroup> IFacRepo.GetCourseWorkGroups => _ctx.WorkGroups;

        IQueryable<SpInstrument> IFacRepo.GetSpInstrument => _ctx.SpInstruments;

        //List<int> IFacRepo.CanWgPublish(List<int> wgIds)
        //{
        //    return _ctx.WorkGroups
        //        .Where(grp => wgIds.Contains(grp.Id))
        //        .Where(grp => grp.GroupMembers.Any() && grp.AssignedSpInstr.InventoryCollection.Any())
        //        .Where(grp => grp.GroupMembers.All(crseStudent =>
        //            crseStudent.AssessorSpResponses.Count(r => !r.Assessee.IsDeleted) ==
        //            grp.GroupMembers.Count(gm => !gm.IsDeleted)*grp.AssignedSpInstr.InventoryCollection.Count()))
        //        .Where(grp => grp.GroupMembers.All(crseStudent =>
        //            crseStudent.AssessorStratResponse.Count(r => !r.Assessee.IsDeleted) ==
        //            grp.GroupMembers.Count(gm => !gm.IsDeleted)))
        //        .Select(wg => wg.Id)
        //        .ToList();
        //}

        IQueryable<StudSpComment> IFacRepo.WgComments => _ctx.StudSpComments;

        IQueryable<Course> IFacRepo.CourseMembers(int courseId)
        {
            return _ctx.Courses
                .Where(c => c.Id == courseId)
                .Include(c => c.Students.Select(sic => sic.Student.Person))
                .Include(c => c.Faculty)
                .Include(c => c.WorkGroups)
                .Include(c => c.StudentInCrseGroups);
        }
    }

}
