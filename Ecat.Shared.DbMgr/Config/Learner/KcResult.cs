using System.Data.Entity.ModelConfiguration;
using Ecat.Shared.Core.ModelLibrary.Learner;

namespace Ecat.Shared.DbMgr.Config
{
    public class ConfigKcResult : EntityTypeConfiguration<KcResult>
    {
        public ConfigKcResult()
        {
            HasKey(p => new {p.StudentId, p.CourseId, p.Version});
            HasMany( p => p.Responses)
            .WithOptional(p => p.Result)
            .HasForeignKey(p => new { p.StudentId, p.CourseId, p.Version })
            .WillCascadeOnDelete(false);
        }
    }
}
