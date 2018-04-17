using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.Learner;

namespace Ecat.Shared.DbMgr.Config
{
    public class ConfigStratResponse : EntityTypeConfiguration<StratResponse>
    {
        public ConfigStratResponse()
        {
            HasKey(p => new
            {
                p.AssessorPersonId,
                p.AssesseePersonId,
                p.CourseId,
                p.WorkGroupId,
            });
            
            HasRequired(p => p.Assessor)
                .WithMany(p => p.AssessorStratResponse)
                .HasForeignKey(p => new {p.AssessorPersonId, p.CourseId, p.WorkGroupId})
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Assessee)
                .WithMany(p => p.AssesseeStratResponse)
                .HasForeignKey(p => new { p.AssesseePersonId, p.CourseId, p.WorkGroupId })
                .WillCascadeOnDelete(false);

        }
    }
}
