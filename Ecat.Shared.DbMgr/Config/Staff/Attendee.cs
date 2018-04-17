using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.Staff.MeetingTaker;

namespace Ecat.Shared.DbMgr.Config
{
    public class ConfigAttendee : EntityTypeConfiguration<MeetingAttendee>
    {
        public ConfigAttendee()
        {
            HasKey(p => new { p.AttendeeId, p.MeetingId });
            HasRequired(p => p.Attendee)
                .WithMany(p => p.MeetingAttendances)
                .HasForeignKey(p => p.AttendeeId)
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Meeting)
                .WithMany(p => p.Attendees)
                .HasForeignKey(p => p.MeetingId)
                .WillCascadeOnDelete(false);
        }
    }
}
