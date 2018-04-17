using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Shared.Core.ModelLibrary.User;
using Newtonsoft.Json.Linq;

namespace Ecat.StudMod.Core
{
    public interface IStudLogic
    {   
        Person StudentPerson { get; set; }
        SaveResult ClientSave(JObject saveBundle);
        Task<List<Course>> GetCourses(int? crseId);
        Task<SpResult> GetWrkGrpResult(int wgId, bool addInstrument);
        Task<WorkGroup> GetWorkGroup(int wgId, bool addInstrument);
        string GetMetadata { get; }
    }
}
    