using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Shared.Core.ModelLibrary.School;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Learner
{
    [TsClass(Module = "ecat.entity.s.learner")]

    public class StudSpCommentFlag : IWorkGroupMonitored, ICourseMonitored
    {
        public string MpAuthor { get; set; }
        public string MpRecipient { get; set; }
        public string MpFaculty { get; set; }

        public int AuthorPersonId { get; set; }
        public int RecipientPersonId { get; set; }
        public int? FlaggedByFacultyId { get; set; }
        public int CourseId { get; set; }
        public int WorkGroupId { get; set; }
        public StudSpComment Comment { get; set; }
        public FacultyInCourse FlaggedByFaculty { get; set; }
        
    }
}
