using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.Learner;

namespace Ecat.Shared.DbMgr.Config
{
    public class ConfigKcResponse : EntityTypeConfiguration<KcResponse>
    {
        public ConfigKcResponse()
        {
            HasKey(p => new {p.StudentId, p.CourseId,p.InventoryId, p.Version});

            Ignore(p => p.Result);

            HasRequired(p => p.Student)
                .WithMany()
                .HasForeignKey(p => new {p.StudentId, p.CourseId})
                .WillCascadeOnDelete(false);
        }
    }
}
