using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.Designer;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Learner
{
    [TsClass(Module = "ecat.entity.s.learner")]
    public class KcResult
    {
        public int InventoryId { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public int Version { get; set; }

        public int InstrumentId { get; set; }
        public int NumberCorrect { get; set; }
        public float Score { get; set; }
        public KcInstrument Instrument { get; set; }
        public ICollection<KcResponse> Responses { get; set; }
    }
}
