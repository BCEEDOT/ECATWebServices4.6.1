﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Breeze.ContextProvider;
//using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Data.Models.Designer;
//using Ecat.Shared.Core.ModelLibrary.Faculty;
using Ecat.Data.Models.Faculty;
//using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Data.Models.Student;
//using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Data.Models.School;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
using Newtonsoft.Json.Linq;

namespace Ecat.Business.Repositories.Interface
{
    public interface IFacRepo
    {
        Person FacultyPerson { get; set; }
        SaveResult ClientSave(JObject saveBundle);
        string GetMetadata { get; }
        Task<List<Course>> GetActiveCourse(int? courseId = null);
        Task<WorkGroup> GetActiveWorkGroup(int courseId, int workGroupId);
        Task<SpInstrument> GetSpInstrument(int instrumentId);
        Task<List<StudSpComment>> GetStudSpComments(int courseId, int workGroupId);
        Task<List<FacSpComment>> GetFacSpComment(int courseId, int workGroupId);
        Task<WorkGroup> GetSpResult(int courseId, int workGroupId);
        Task<List<WorkGroup>> GetRoadRunnerWorkGroups(int courseId);

        Task<Course> GetAllCourseMembers(int courseId);
    }
}
