using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.School;
using TypeLite;
using Ecat.Shared.Core.Interface;

namespace Ecat.Shared.Core.ModelLibrary.User
{
    [TsClass(Module = "ecat.entity.s.user")]
    public class ProfileStudent : IProfileBase
    {
        public int PersonId { get; set; }
        public string Bio { get; set; }
        public string HomeStation { get; set; }
        public Person Person { get; set; }
        public string ContactNumber { get; set; }
        public string Commander { get; set; }
        public string Shirt { get; set; }
        public string CommanderEmail { get; set; }
        public string ShirtEmail { get; set; }

        public ICollection<StudentInCourse> Courses { get; set; } 
        public ICollection<CrseStudentInGroup> CourseGroupMemberships { get; set; } 
    }
}
