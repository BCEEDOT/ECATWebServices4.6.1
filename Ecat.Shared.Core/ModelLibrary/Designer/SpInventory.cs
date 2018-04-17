using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.Learner;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Designer
{
    [TsClass(Module = "ecat.entity.s.designer")]
    public class SpInventory : IAuditable, IInventory<SpInstrument>
    {
        public int Id { get; set; }
        public int InstrumentId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsScored { get; set; }
        public bool IsDisplayed { get; set; }
        public string Behavior { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public SpInstrument Instrument { get; set; }
        public ICollection<SpResponse> ItemResponses { get; set; } 
    }
}
