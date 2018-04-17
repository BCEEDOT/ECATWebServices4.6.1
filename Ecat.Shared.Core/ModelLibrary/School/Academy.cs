using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.Utility;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.School
{
    [TsClass(Module = "ecat.entity.s.school")]
    public class Academy
    {
        public string Id { get; set; }
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string MpEdLevel { get; set; }
        public AcademyBase Base { get; set; }
        public string BbCategoryId { get; set; }
        public string ParentBbCategoryId { get; set; }
    }
}
