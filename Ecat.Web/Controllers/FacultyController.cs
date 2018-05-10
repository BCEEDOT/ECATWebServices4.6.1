using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Breeze.ContextProvider;
//using Ecat.FacMod.Core;
//using Ecat.Shared.Core.ModelLibrary.Designer;
//using Ecat.Shared.Core.ModelLibrary.Faculty;
//using Ecat.Shared.Core.ModelLibrary.Learner;
//using Ecat.Shared.Core.ModelLibrary.School;
//using Ecat.Shared.Core.ModelLibrary.User;
//using Ecat.Shared.Core.Utility;
//using Ecat.Shared.DbMgr.Context;
using Ecat.Data.Static;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Faculty;
using Ecat.Data.Models.Student;
using Ecat.Web.Utility;
using Newtonsoft.Json.Linq;

namespace Ecat.Web.Controllers
{
    [EcatRolesAuthorized(Is = new[] { RoleMap.Faculty })]
    public class FacultyController : EcatBaseBreezeController
    {
        //private readonly IFacLogic _facLogic;
        private readonly IFacRepo _facRepo;

        public FacultyController(IFacRepo facRepo)
        {
            _facRepo = facRepo;
        }

        internal override void SetUser(Person person)
        {
            _facRepo.FacultyPerson = person;
        }

        [HttpGet]
        [AllowAnonymous]
        public string Metadata()
        {
            return _facRepo.GetMetadata;
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _facRepo.ClientSave(saveBundle);
        }

        [HttpGet]
        public async Task<List<Course>> GetCourses()
        {
            return await _facRepo.GetActiveCourse();
        }

        [HttpGet]
        public async Task<List<Course>> ActiveCourse(int courseId)
        {
            return await _facRepo.GetActiveCourse(courseId);
        }

        [HttpGet]
        public async Task<WorkGroup> ActiveWorkGroup(int courseId, int workGroupId)
        {
            return await _facRepo.GetActiveWorkGroup(courseId, workGroupId);
        }

        [HttpGet]
        public async Task<List<WorkGroup>> GetRoadRunnerWorkGroups(int courseId)
        {
            return await _facRepo.GetRoadRunnerWorkGroups(courseId);
        }

        [HttpGet]
        public async Task<SpInstrument> SpInstrument(int instrumentId)
        {
            return await _facRepo.GetSpInstrument(instrumentId);
        }

        [HttpGet]
        public async Task<List<StudSpComment>> ActiveWgSpComment(int courseId, int workGroupId)
        {
            return await _facRepo.GetStudSpComments(courseId, workGroupId);
        }

        [HttpGet]
        public async Task<List<FacSpComment>> ActiveWgFacComment(int courseId, int workGroupId)
        {
            return await _facRepo.GetFacSpComment(courseId, workGroupId);
        }

        [HttpGet]
        public async Task<WorkGroup> ActiveWgSpResult(int courseId, int workGroupId)
        {
            return await _facRepo.GetSpResult(courseId, workGroupId);
        }

        [HttpGet]
        public async Task<Course> GetAllCourseMembers(int courseId)
        {
            return await _facRepo.GetAllCourseMembers(courseId);
        }

    }
}
