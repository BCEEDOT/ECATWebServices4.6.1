using System.Threading.Tasks;
using System.Web.Http;
using Breeze.ContextProvider;
//using Ecat.Shared.Core.ModelLibrary.User;
//using Ecat.Shared.Core.ModelLibrary.Designer;
//using Ecat.Shared.Core.Provider;
//using Ecat.UserMod.Core;
using Ecat.Data.Models.User;
using Ecat.Business.Repositories.Interface;
using Ecat.Business.Utilities;
using Ecat.Data.Models.Designer;
using Ecat.Web.Utility;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Ecat.Web.Controllers
{
    [EcatRolesAuthorized]
    public class UserController : EcatBaseBreezeController
    {
        //private readonly IUserLogic _userLogic;
        private readonly IUserRepo _userRepo;

        public UserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        internal override void SetUser(Person person)
        {
            _userRepo.User = person;
        }

        [HttpGet]
        [AllowAnonymous]
        public string Metadata()
        {
            return _userRepo.Metadata;
        }

        //TODO: Remove for production
        [HttpGet]
        public IEnumerable<Person> GetUsers()
        {
            return _userRepo.GetUsers();
        }

        [HttpPost]
        [AllowAnonymous]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _userRepo.ClientSave(saveBundle);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> CheckUserEmail(string email)
        {
            var emailChecker = new ValidEmailChecker();
            return !emailChecker.IsValidEmail(email) && await _userRepo.UniqueEmailCheck(email);
        }

        [HttpGet]
        public async Task<object> Profiles()
        {
            return await _userRepo.GetProfile();
        }

        [HttpGet]
        public async Task<CogInstrument> GetCogInst(string type)
        {
            return await _userRepo.GetCogInst(type);
        }

        [HttpGet]
        public async Task<List<object>> GetCogResults(bool? all)
        {
            return await _userRepo.GetCogResults(all);
	}

	[HttpGet]
        public async Task<List<RoadRunner>> RoadRunnerInfos()
        {
            return await _userRepo.GetRoadRunnerInfo();
        }

    }
}
