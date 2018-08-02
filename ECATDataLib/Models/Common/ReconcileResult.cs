﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Data.Models.School;
//using Ecat.Shared.Core.ModelLibrary.User;
using TypeLite;

namespace Ecat.Data.Models.Common
{

    public abstract class ReconcileResult 
    {
        public Guid Id { get; set; }
        public string AcademyId { get; set; }
        public int NumAdded { get; set; }
        public int NumRemoved { get; set; }
        public bool IsAuthorized { get; set; }
        public bool HasToken { get; set; }
    }

    [TsClass(Module = "ecat.entity.s.lmsAdmin")]
    public class CourseReconResult : ReconcileResult
    {
        public ICollection<Course> Courses { get; set; }
    }

    [TsClass(Module = "ecat.entity.s.lmsAdmin")]
    public class GroupReconResult : ReconcileResult
    {
        public ICollection<WorkGroup> Groups { get; set; }
    }

    [TsClass(Module = "ecat.entity.s.lmsAdmin")]
    public class MemReconResult : ReconcileResult
    {
        public int CourseId { get; set; }
        public int NumOfAccountCreated { get; set; }
        public ICollection<FacultyInCourse> Faculty { get; set; }
        public ICollection<StudentInCourse> Students { get; set; }
        public ICollection<int> RemovedIds { get; set; }
    }

    [TsClass(Module = "ecat.entity.s.lmsAdmin")]
    public class GroupMemReconResult : ReconcileResult
    {
        public int CourseId { get; set; }
        public int WorkGroupId { get; set; }
        public string WorkGroupName { get; set; }
        public string GroupType { get; set; }
        public ICollection<CrseStudentInGroup> GroupMembers { get; set; }
    }

    public class CourseDetailsReconResult : ReconcileResult
    {
        public GroupReconResult GroupReconResult { get; set; }
        public MemReconResult CourseMemberReconResult { get; set; }
        public List<GroupMemReconResult> GroupMemReconResults { get; set; }
        public bool GroupReconSuccess { get; set; }
        public bool MemReconSuccess { get; set; }
        public bool GroupMemReconSuccess { get; set; }
        public string ErrorMessage { get; set; }

    }
}
