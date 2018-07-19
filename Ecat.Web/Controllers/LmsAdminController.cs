using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Breeze.ContextProvider;
//using Ecat.FacMod.Core;
//using Ecat.LmsAdmin.Mod;
//using Ecat.Shared.Core.ModelLibrary.Common;
//using Ecat.Shared.Core.ModelLibrary.School;
//using Ecat.Shared.Core.ModelLibrary.User;
//using Ecat.Shared.Core.Utility;
//using Ecat.Shared.DbMgr.Context;
using Ecat.Data.Static;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Models.User;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Common;
using Ecat.Data.Models.Designer;
using Ecat.Web.Utility;
using Newtonsoft.Json.Linq;

namespace Ecat.Web.Controllers
{
    [EcatRolesAuthorized(Is = new[] { RoleMap.Faculty, RoleMap.CrseAdmin })]
    public class LmsAdminController : EcatBaseBreezeController
    {
        private readonly ILmsAdminCourseOps _lmsCourseOps;
        private readonly ILmsAdminGroupOps _lmsGroupOps;
        private readonly ILmsAdminTokenRepo _lmsTokenRepo;

        public LmsAdminController(ILmsAdminCourseOps lmsCourseOps, ILmsAdminGroupOps lmsGroupOps, ILmsAdminTokenRepo lmsTokenRepo)
        {
            _lmsCourseOps = lmsCourseOps;
            _lmsGroupOps = lmsGroupOps;
            _lmsTokenRepo = lmsTokenRepo;
        }

        [HttpGet]
        public string Metadata()
        {
            return _lmsCourseOps.Metadata;
        }

        internal override void SetUser(Person person)
        {
            _lmsCourseOps.Faculty = person.Faculty;
            _lmsGroupOps.Faculty = person.Faculty;
            _lmsTokenRepo.Faculty = person.Faculty;
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _lmsCourseOps.SaveClientChanges(saveBundle);
        }

        [HttpGet]
        public async Task<List<Course>> GetAllCourses()
        {
            return await _lmsCourseOps.GetAllCourses();
        }

        [HttpGet]
        public async Task<Course> GetAllCourseMembers(int courseId)
        {
            return await _lmsCourseOps.GetAllCourseMembers(courseId);
        }

        [HttpGet]
        public async Task<Course> GetAllGroups(int courseId)
        {
            return await _lmsGroupOps.GetCourseGroups(courseId);
        }

        [HttpGet]
        public async Task<WorkGroup> GetGroupMembers(int workGroupId)
        {
            return await _lmsGroupOps.GetWorkGroupMembers(workGroupId);
        }

        [HttpGet]
        public async Task<CourseReconResult> PollCourses()
        {
            return await _lmsCourseOps.ReconcileCourses();
        }

        [HttpGet]
        public async Task<MemReconResult> PollCourseMembers(int courseId)
        {
            return await _lmsCourseOps.ReconcileCourseMembers(courseId);
        }

        [HttpGet]
        public async Task<MemReconResult> PollCanvasCourseMembers(int courseId)
        {

            var token = await _lmsTokenRepo.CheckCanvasTokenInfo();
            var result = new MemReconResult();
            if (!token)
            {
                result.HasToken = false;
                return result;
            }

            result = await _lmsCourseOps.PollCanvasCourseMems(courseId);
            return result;

        }

        [HttpGet]
        public async Task<CourseReconResult> PollCanvasCourses()
        {
            var token = await _lmsTokenRepo.CheckCanvasTokenInfo();
            var result = new CourseReconResult();
            if (!token)
            {
                result.HasToken = false;
                return result;
            }

            result = await _lmsCourseOps.PollCanvasCourses();
            return result;
        }

        [HttpGet]
        public async Task<GroupReconResult> PollGroups(int courseId)
        {
            return await _lmsGroupOps.ReconcileGroups(courseId);
        }

        [HttpGet]
        public async Task<GroupMemReconResult> PollGroupMembers(int workGroupId)
        {
            return await _lmsGroupOps.ReconcileGroupMembers(workGroupId);
        }

        //[HttpGet]
        //public async Task<List<GroupMemReconResult>> PollGroupCategory(int courseId, string category)
        //{
        //    return await _lmsGroupOps.ReconcileAllGroupMembers(courseId);
        //    //return await _lmsGroupOps.ReconcileGroupMembers(courseId, category);
        //}
        //ECAT 2.0 only get BC1 groups from LMS
        [HttpGet]
        public async Task<List<GroupMemReconResult>> PollGroupCategory(int courseId)
        {
            return await _lmsGroupOps.ReconcileAllGroupMembers(courseId);
        }

        [HttpGet]
        public async Task<SaveGradeResult> SyncBbGrades(int crseId, string wgCategory) {
            return await _lmsGroupOps.SyncBbGrades(crseId, wgCategory);
        }

        [HttpGet]
        public async Task<List<WorkGroupModel>> GetCourseModels(int courseId)
        {
            return await _lmsCourseOps.GetCourseModels(courseId);
        }

        [HttpGet]
        public async Task<List<WorkGroup>> GetAllGroupSetMembers(int courseId, string categoryId)
        {
            return await _lmsGroupOps.GetAllGroupSetMembers(courseId, categoryId);
        }
    }
}
