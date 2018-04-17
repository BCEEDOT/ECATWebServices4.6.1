using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData.Query;
using Breeze.ContextProvider;
using Breeze.WebApi2;
using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.Core.Utility;
using Ecat.StudMod.Core;
using Ecat.Web.Utility;
using Newtonsoft.Json.Linq;

namespace Ecat.Web.Controllers
{
    [EcatRolesAuthorized(Is = new[] { RoleMap.Student })]
    public class StudentController : EcatBaseBreezeController
    {
        private readonly IStudLogic _studLogic;

        public StudentController(IStudLogic studLogic)
        {
            _studLogic = studLogic;
        }

        internal override void SetUser(Person person)
        {
            _studLogic.StudentPerson = person;
        }

        [HttpGet]
        public string Metadata()
        {
            return _studLogic.GetMetadata;
        }

        [HttpGet]
        public Task<List<Course>> GetCourses()
        {
            return _studLogic.GetCourses(null);
        }

        [HttpGet]
        public Task<List<Course>> ActiveCourse(int crseId)
        {
            return _studLogic.GetCourses(crseId);
        }

        [HttpGet]
        public async Task<WorkGroup> ActiveWorkGroup(int wgId, bool addAssessment)
        {
            return await _studLogic.GetWorkGroup(wgId, addAssessment);
        }

        [HttpGet]
        public async Task<SpResult> GetMyWgResult(int wgId, bool addInstrument)
        {
            return await _studLogic.GetWrkGrpResult(wgId, addInstrument);
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _studLogic.ClientSave(saveBundle);
        }
    }
}
