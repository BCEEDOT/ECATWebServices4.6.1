using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Breeze.ContextProvider;
using Ecat.FacMod.Core;
using Ecat.LmsAdmin.Mod;
using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.Core.Utility;
using Ecat.Shared.DbMgr.Context;
using Ecat.Web.Utility;
using Newtonsoft.Json.Linq;

namespace Ecat.Web.Controllers
{
    [EcatRolesAuthorized(Is = new[] { RoleMap.Faculty, RoleMap.CrseAdmin })]
    public class LmsAdminController : EcatBaseBreezeController
    {
        private readonly ILmsAdminCourseOps _lmsCourseOps;
        private readonly ILmsAdminGroupOps _lmsGroupOps;
        public LmsAdminController(ILmsAdminCourseOps lmsCourseOps, ILmsAdminGroupOps lmsGroupOps)
        {
            _lmsCourseOps = lmsCourseOps;
            _lmsGroupOps = lmsGroupOps;
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
        public async Task<GroupReconResult> PollGroups(int courseId)
        {
            return await _lmsGroupOps.ReconcileGroups(courseId);
        }

        [HttpGet]
        public async Task<GroupMemReconResult> PollGroupMembers(int workGroupId)
        {
            return await _lmsGroupOps.ReconcileGroupMembers(workGroupId);
        }

        [HttpGet]
        public async Task<List<GroupMemReconResult>> PollGroupCategory(int courseId, string category)
        {
            return await _lmsGroupOps.ReconcileGroupMembers(courseId, category);
        }

        [HttpGet]
        public async Task<SaveGradeResult> SyncBbGrades(int crseId, string wgCategory) {
            return await _lmsGroupOps.SyncBbGrades(crseId, wgCategory);
        }
    }
}
