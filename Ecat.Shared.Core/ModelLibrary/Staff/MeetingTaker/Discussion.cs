using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Staff.MeetingTaker
{
    [TsClass(Module = "ecat.entity.s.staff")]
    public class Discussion
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }  
        public string DiscussionItem { get; set; }

        public Meeting Meeting { get; set; }
    }
}
