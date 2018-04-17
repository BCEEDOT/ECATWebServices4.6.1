using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Shared.Core.ModelLibrary.School;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Designer
{
    [TsClass(Module = "ecat.entity.s.designer")]
    public class KcInstrument : IInstrument
    {
        public int Id { get; set; }
        public int? ModifiedById { get; set; }
        public string Instructions { get; set; }
        public string Version { get; set; }
        public bool IsActive { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public ICollection<KcResult> Results { get; set; }
        public ICollection<WorkGroup> AssignedGroups { get; set; }
    }
}
