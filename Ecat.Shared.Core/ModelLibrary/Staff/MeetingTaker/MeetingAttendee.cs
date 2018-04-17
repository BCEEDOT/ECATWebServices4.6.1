using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.User;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Staff.MeetingTaker
{
    [TsClass(Module = "ecat.entity.s.staff")]
    public class MeetingAttendee
    {
        public int MeetingId { get; set; }
        public int AttendeeId { get; set; }
        public int IsOrganizer { get; set; }

        public ProfileStaff Attendee { get; set; }
        public Meeting Meeting { get; set; }
    }
}
