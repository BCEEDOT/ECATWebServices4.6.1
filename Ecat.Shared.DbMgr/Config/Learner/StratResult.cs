using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.Learner;

namespace Ecat.Shared.DbMgr.Config
{
    public class ConfigStratResult : EntityTypeConfiguration<StratResult>
    {
        public ConfigStratResult()
        {
            HasKey(p => new {p.StudentId, p.CourseId, p.WorkGroupId});
            Property(p => p.StudStratAwardedScore).HasPrecision(18, 3);
            Property(p => p.FacStratAwardedScore).HasPrecision(18, 3);
            Ignore(p => p.StratResponses);
            Ignore(p => p.FacStrat);

            HasRequired(p => p.Course)
                .WithMany(p => p.StratResults)
                .HasForeignKey(p => p.CourseId)
                .WillCascadeOnDelete(false);
        }
    }
}
