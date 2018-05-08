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
//using Ecat.Shared.Core.ModelLibrary.Learner;
//using Ecat.Shared.Core.ModelLibrary.School;
//using Ecat.Shared.Core.ModelLibrary.User;
//using Ecat.Shared.Core.Utility;
//using Ecat.StudMod.Core;
using Ecat.Data.Static;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Models.User;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Student;
using Ecat.Web.Utility;
using Newtonsoft.Json.Linq;

namespace Ecat.Web.Controllers
{
    [EcatRolesAuthorized(Is = new[] { RoleMap.Student })]
    public class StudentController : EcatBaseBreezeController
    {
        //private readonly IStudLogic _studLogic;
        private readonly IStudRepo _studRepo;

        public StudentController(IStudRepo studRepo)
        {
            _studRepo = studRepo;
        }

        internal override void SetUser(Person person)
        {
            _studRepo.StudentPerson = person;
        }

        [HttpGet]
        public string Metadata()
        {
            return _studRepo.GetMetadata;
        }

        [HttpGet]
        public Task<List<Course>> GetCourses()
        {
            return _studRepo.GetCourses(null);
        }

        [HttpGet]
        public Task<List<Course>> ActiveCourse(int crseId)
        {
            return _studRepo.GetCourses(crseId);
        }

        [HttpGet]
        public async Task<WorkGroup> ActiveWorkGroup(int wgId, bool addAssessment)
        {
            return await _studRepo.GetWorkGroup(wgId, addAssessment);
        }

        [HttpGet]
        public async Task<SpResult> GetMyWgResult(int wgId, bool addInstrument)
        {
            return await _studRepo.GetWrkGrpResult(wgId, addInstrument);
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _studRepo.ClientSave(saveBundle);
        }
    }
}
