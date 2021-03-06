﻿using System.Data.Entity.ModelConfiguration;
//using Ecat.Shared.Core.ModelLibrary.Faculty;

namespace Ecat.Data.Models.Faculty.Config
{
    public class ConfigFacSpCommentFlag : EntityTypeConfiguration<FacSpCommentFlag>
    {
        public ConfigFacSpCommentFlag()
        {
            HasKey(p => new
            {
                p.RecipientPersonId,
                p.CourseId,
                p.WorkGroupId
            });

            HasRequired(p => p.Comment)
                .WithOptional(p => p.Flag);
        }
    }
}
