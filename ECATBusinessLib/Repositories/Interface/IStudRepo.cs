using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Breeze.ContextProvider;
//using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Data.Models.Student;
//using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Data.Models.School;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
using Newtonsoft.Json.Linq;

namespace Ecat.Business.Repositories.Interface
{
    public interface IStudRepo
    {   
        Person StudentPerson { get; set; }
        SaveResult ClientSave(JObject saveBundle);
        Task<List<Course>> GetCourses(int? crseId);
        Task<SpResult> GetWrkGrpResult(int wgId, bool addInstrument);
        Task<WorkGroup> GetWorkGroup(int wgId, bool addInstrument);
        string GetMetadata { get; }
    }
}
    