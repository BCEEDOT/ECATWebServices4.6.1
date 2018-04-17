using System.Threading.Tasks;
using System.Web.Http;
using Breeze.ContextProvider;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Shared.Core.Provider;
using Ecat.UserMod.Core;
using Ecat.Web.Utility;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Ecat.Web.Controllers
{
    [EcatRolesAuthorized]
    public class UserController : EcatBaseBreezeController
    {
        private readonly IUserLogic _userLogic;

        public UserController(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        internal override void SetUser(Person person)
        {
            _userLogic.User = person;
        }

        [HttpGet]
        [AllowAnonymous]
        public string Metadata()
        {
            return _userLogic.Metadata;
        }

        [HttpPost]
        [AllowAnonymous]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _userLogic.ClientSave(saveBundle);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> CheckUserEmail(string email)
        {
            var emailChecker = new ValidEmailChecker();
            return !emailChecker.IsValidEmail(email) && await _userLogic.UniqueEmailCheck(email);
        }

        [HttpGet]
        public async Task<object> Profiles()
        {
            return await _userLogic.GetProfile();
        }

        [HttpGet]
        public async Task<CogInstrument> GetCogInst(string type)
        {
            return await _userLogic.GetCogInst(type);
        }

        [HttpGet]
        public async Task<List<object>> GetCogResults(bool? all)
        {
            return await _userLogic.GetCogResults(all);
	}

	[HttpGet]
        public async Task<List<RoadRunner>> RoadRunnerInfos()
        {
            return await _userLogic.GetRoadRunnerInfo();
        }

    }
}
