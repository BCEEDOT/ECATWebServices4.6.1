using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.School;
using TypeLite;
using Ecat.Shared.Core.Interface;

namespace Ecat.Shared.Core.ModelLibrary.User
{
    [TsClass(Module = "ecat.entity.s.user")]
    public class ProfileFaculty : IProfileBase
    {
        public int PersonId { get; set; }
        public string Bio { get; set; }
        public string HomeStation { get; set; }
        public Person Person { get; set; }
        public bool IsCourseAdmin { get; set; }
        public bool IsReportViewer   { get; set; }
        public string AcademyId { get; set; }
        public ICollection<FacultyInCourse> Courses { get; set; }
    }
}
