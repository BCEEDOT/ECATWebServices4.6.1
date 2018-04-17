using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.Interface;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Designer
{
    [TsClass(Module = "ecat.entity.s.designer")]
    public class CogInventory : IInventory<CogInstrument>, IAuditable
    {
        public int Id { get; set; }
        public int InstrumentId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsScored { get; set; }
        public bool IsDisplayed { get; set; }
        public string AdaptiveDescription { get; set; }
        public string InnovativeDescription { get; set; }
        public string ItemType { get; set; }
        public string ItemDescription { get; set; }
        public bool? IsReversed { get; set; }

        public CogInstrument Instrument { get; set; }
        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }


    }
}
