using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Breeze.ContextProvider;
using Ecat.FacMod.Core;
using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Shared.Core.ModelLibrary.Faculty;
using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.Core.Utility;
using Ecat.Shared.DbMgr.Context;
using Ecat.Web.Utility;
using Newtonsoft.Json.Linq;

namespace Ecat.Web.Controllers
{
    [EcatRolesAuthorized(Is = new[] { RoleMap.Faculty })]
    public class FacultyController : EcatBaseBreezeController
    {
        private readonly IFacLogic _facLogic;

        public FacultyController(IFacLogic facLogic)
        {
            _facLogic = facLogic;
        }

        internal override void SetUser(Person person)
        {
            _facLogic.FacultyPerson = person;
        }

        [HttpGet]
        [AllowAnonymous]
        public string Metadata()
        {
            return _facLogic.GetMetadata;
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _facLogic.ClientSave(saveBundle);
        }

        [HttpGet]
        public async Task<List<Course>> GetCourses()
        {
            return await _facLogic.GetActiveCourse();
        }

        [HttpGet]
        public async Task<List<Course>> ActiveCourse(int courseId)
        {
            return await _facLogic.GetActiveCourse(courseId);
        }

        [HttpGet]
        public async Task<WorkGroup> ActiveWorkGroup(int courseId, int workGroupId)
        {
            return await _facLogic.GetActiveWorkGroup(courseId, workGroupId);
        }

        [HttpGet]
        public async Task<List<WorkGroup>> GetRoadRunnerWorkGroups(int courseId)
        {
            return await _facLogic.GetRoadRunnerWorkGroups(courseId);
        }

        [HttpGet]
        public async Task<SpInstrument> SpInstrument(int instrumentId)
        {
            return await _facLogic.GetSpInstrument(instrumentId);
        }

        [HttpGet]
        public async Task<List<StudSpComment>> ActiveWgSpComment(int courseId, int workGroupId)
        {
            return await _facLogic.GetStudSpComments(courseId, workGroupId);
        }

        [HttpGet]
        public async Task<List<FacSpComment>> ActiveWgFacComment(int courseId, int workGroupId)
        {
            return await _facLogic.GetFacSpComment(courseId, workGroupId);
        }

        [HttpGet]
        public async Task<WorkGroup> ActiveWgSpResult(int courseId, int workGroupId)
        {
            return await _facLogic.GetSpResult(courseId, workGroupId);
        }

    }
}
