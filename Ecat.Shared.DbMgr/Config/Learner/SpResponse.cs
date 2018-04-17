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
    public class ConfigSpResponse : EntityTypeConfiguration<SpResponse>
    {
        public ConfigSpResponse()
        {
            HasKey(p => new
            {
                p.AssessorPersonId,
                p.AssesseePersonId,
                p.CourseId,
                p.WorkGroupId,
                p.InventoryItemId
            });

            HasRequired(p => p.Assessor)
                .WithMany(p => p.AssessorSpResponses)
                .HasForeignKey(p => new {p.AssessorPersonId, p.CourseId, p.WorkGroupId})
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Assessee)
                .WithMany(p => p.AssesseeSpResponses)
                .HasForeignKey(p => new {p.AssesseePersonId, p.CourseId, p.WorkGroupId})
                .WillCascadeOnDelete(false);

            HasRequired(p => p.InventoryItem)
                .WithMany(p => p.ItemResponses)
                .HasForeignKey(p => p.InventoryItemId)
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Course)
                .WithMany(p => p.SpResponses)
                .HasForeignKey(p => p.CourseId)
                .WillCascadeOnDelete(false);

            HasRequired(p => p.WorkGroup)
                .WithMany(p => p.SpResponses)
                .HasForeignKey(p => p.WorkGroupId)
                .WillCascadeOnDelete(false);
        }
    }

}
