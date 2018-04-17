using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Staff.MeetingTaker
{
    [TsClass(Module = "ecat.entity.s.staff")]
    public class Decision
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public string Status { get; set; }
        public string DecisionItem { get; set; }
        public string ApprovalAuthority { get; set; }
        public DateTime ApprovedDate { get; set; }
        public Meeting Meeting { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
