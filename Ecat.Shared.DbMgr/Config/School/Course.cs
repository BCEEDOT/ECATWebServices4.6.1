using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.School;

namespace Ecat.Shared.DbMgr.Config
{
    public class ConfigCourse : EntityTypeConfiguration<Course>
    {
        public ConfigCourse()
        {
            Property(p => p.BbCourseId)
            .HasMaxLength(60)
            .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                new IndexAnnotation(new IndexAttribute("IX_UniqueBbCourseId") { IsUnique = true }));


        }
    }
}
