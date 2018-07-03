using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Ecat.Shared.Core.ModelLibrary.Learner;

namespace Ecat.Data.Models.Student.Config
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

            HasRequired(response => response.Assessor)
                .WithMany(crseStuInGroup => crseStuInGroup.AssessorSpResponses)
                .HasForeignKey(response => new { response.AssessorPersonId, response.CourseId, response.WorkGroupId})
                .WillCascadeOnDelete(false);

            HasRequired(response => response.Assessee)
                .WithMany(crseStuInGroup => crseStuInGroup.AssesseeSpResponses)
                .HasForeignKey(response => new {response.AssesseePersonId, response.CourseId, response.WorkGroupId})
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
