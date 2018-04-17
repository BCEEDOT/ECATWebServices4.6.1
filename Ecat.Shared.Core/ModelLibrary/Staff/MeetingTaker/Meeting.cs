using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Staff.MeetingTaker
{
    [TsClass(Module = "ecat.entity.s.staff")]
    public class Meeting
    {
        public int Id { get; set; }
        public string MeetingReason { get; set; }
        public string Abstract { get; set; }
        public string Background { get; set; }
        public string Purpose { get; set; }

        public DateTime MeetingDate { get; set; }
        public ICollection<MeetingAttendee> Attendees { get; set; }
        public ICollection<ActionItem> ActionItemses { get; set; }
        public ICollection<Decision> Decisions { get; set; }
        public ICollection<Discussion> Discussions { get; set; }
    }
}
