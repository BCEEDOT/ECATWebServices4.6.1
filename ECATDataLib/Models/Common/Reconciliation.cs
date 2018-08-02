using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Canvas;
//using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Data.Models.School;
using Ecat.Data.Models.User;
using TypeLite;

namespace Ecat.Data.Models.Common
{

    public class CourseReconcile
    {
        public Course Course { get; set; }
        public ICollection<UserReconcile> FacultyToReconcile { get; set; }
        public ICollection<UserReconcile> StudentsToReconcile { get; set; }
    }

    public class UserReconcile
    {
        public int PersonId { get; set; }
        public string BbUserId { get; set; }
        public string CourseMemId { get; set; }
        public Person Person { get; set; }
        public bool CanDelete { get; set; }
        public bool IsMarkedDeleted { get; set; }
        public bool NewEnrollment { get; set; }
        public bool RemoveEnrollment { get; set; }
        public bool FlagDeleted { get; set; }
        public bool UnFlagDeleted { get; set; }
    }

    public class WorkgroupAndMemberReconcile
    {
        public string BbCrseId { get; set; }
        public int CrseId { get; set; }
        public ICollection<WorkgroupReconcile> WorkGroups { get; set; }
    }

    public class WorkgroupReconcile
    {
        public int WgId { get; set; }
        public string BbWgId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public bool HasMembers { get; set; }
        public bool NeedToAdd { get; set; }
        public GroupMemReconResult ReconResult { get; set; }
        public ICollection<WorkgroupMemberReconcile> Members { get; set; }
    }

    public class WorkgroupMemberReconcile
    {
        public int StudentId { get; set; }
        //public string BbGroupMemId { get; set; }
        public string BbUserId { get; set; }
        //public string BbCrseMemId { get; set; }
        public int WorkGroupId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsMoving { get; set; }
        public bool NewEnrollment { get; set; }
        public bool RemoveEnrollment { get; set; }
        public bool HasChildren { get; set; }
        public StudentOnTheMoveReconcile StudentOnTheMoveReconcile { get; set; }
        public CanvasUser CanvasUser { get; set; }
        public CrseStudentInGroup CrseStudentInGroup { get; set; }
        //public GmrMemberVo BbGmVo { get; set; }
    }

    public class GmrMemberVo
    {
        public string CourseMembershipId { get; set; }
        public string GroupId { get; set; }
        public string GroupMembershipId { get; set; }
    }

    public class StudentOnTheMoveReconcile
    {
        public int StudentId { get; set; }
        public bool HasChildren { get; set; }
        public int CourseId { get; set; }
        public int ToWorkGroupId { get; set; }
        public int FromWorkGroupId { get; set; }
    }

    public class StudentOnTheMove
    {
        public CrseStudentInGroup Student { get; set; }
        public int StudentId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsMoving { get; set; }
        public bool HasChildren { get; set; }
        public int CourseId { get; set; }
        public int ToWorkGroupId { get; set; }
        public int FromWorkGroupId { get; set; }
    }
}
