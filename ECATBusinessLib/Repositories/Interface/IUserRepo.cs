using System.Threading.Tasks;
using Breeze.ContextProvider;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
//using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Data.Models.Designer;
using LtiLibrary.Core.Lti1;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Ecat.Business.Repositories.Interface
{
    public interface IUserRepo
    {
        SaveResult ClientSave(JObject saveBundle);
        Person User { get; set; }
        string Metadata { get; }
        Task<List<object>> GetProfile();
        Task<Person> LoginUser(string userName, string password);
        Task<Person> ProcessLtiUser(ILtiRequest parsedRequest);
        Task<bool> UniqueEmailCheck(string email);
        Task<CogInstrument> GetCogInst(string type);
        Task<List<object>> GetCogResults(bool? all);
        Task<List<RoadRunner>> GetRoadRunnerInfo();
        IEnumerable<Person> GetUsers();
    }
}
