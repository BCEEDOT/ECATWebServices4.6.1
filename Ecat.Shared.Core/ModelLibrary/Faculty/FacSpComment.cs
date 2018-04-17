using System;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Shared.Core.ModelLibrary.School;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Faculty
{
    [TsClass(Module = "ecat.entity.s.faculty")]

    public class FacSpComment : IAuditable, IWorkGroupMonitored, ICourseMonitored
    {
        public string EntityId => $"{RecipientPersonId}|{CourseId}|{WorkGroupId}";
        public int RecipientPersonId { get; set; }
        public int WorkGroupId { get; set; }
        public int FacultyPersonId { get; set; }
        public int CourseId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CommentText { get; set; }


        public FacultyInCourse FacultyCourse { get; set; }
        public CrseStudentInGroup Recipient { get; set; }
        public WorkGroup WorkGroup { get; set; }
        public Course Course { get; set; }
        public FacSpCommentFlag Flag { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
