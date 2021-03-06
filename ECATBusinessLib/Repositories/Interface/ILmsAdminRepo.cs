﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;
//using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Data.Models.Common;
//using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Data.Models.School;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Designer;
using Newtonsoft.Json.Linq;

namespace Ecat.Business.Repositories.Interface
{
    public interface ILmsAdminCourseOps
    {
        string Metadata { get; }
        ProfileFaculty Faculty { get; set; }
        Task<List<Course>> GetAllCourses();
        Task<CourseReconResult> ReconcileCourses();
        Task<Course> GetAllCourseMembers(int courseId);
        Task<MemReconResult> ReconcileCourseMembers(int courseId);
        SaveResult SaveClientChanges(JObject saveBundle);

        Task<List<WorkGroupModel>> GetCourseModels(int courseId);
    }

    public interface ILmsAdminGroupOps
    {
        ProfileFaculty Faculty { get; set; }
        Task<Course> GetCourseGroups(int courseId);
        Task<WorkGroup> GetWorkGroupMembers(int workGroupId);
        Task<GroupReconResult> ReconcileGroups(int courseId);
        Task<GroupMemReconResult> ReconcileGroupMembers(int wgId);
        //Task<List<GroupMemReconResult>> ReconcileGroupMembers(int courseId, string groupCategory);
        Task<List<GroupMemReconResult>> ReconcileAllGroupMembers(int courseId);
        Task<SaveGradeResult> SyncBbGrades(int crseId, string wgCategory);

        Task<List<WorkGroup>> GetAllGroupSetMembers(int courseId, string categoryId);
    }
}
