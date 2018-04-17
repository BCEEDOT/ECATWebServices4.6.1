using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Staff.MeetingTaker
{
    [TsClass(Module = "ecat.entity.s.staff")]
    public class ActionItem
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public string Todo { get; set; }
        public string Opr { get; set; }
        public string MpActionStatus { get; set; }
        public string Resolution { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ResolutionDate { get; set; }

        public Meeting Meeting { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
