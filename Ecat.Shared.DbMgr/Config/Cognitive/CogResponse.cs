using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.Cognitive;
using Ecat.Shared.Core.ModelLibrary.User;

namespace Ecat.Shared.DbMgr.Config
{
    public class ConfigCogResponse : EntityTypeConfiguration<CogResponse>
    {
        public ConfigCogResponse()
        {
            HasKey(p => new
            {
                p.CogInventoryId,
                p.PersonId,
                p.Attempt

            });

            HasRequired(p => p.Person)
                .WithMany()
                .HasForeignKey(p => p.PersonId);

            HasRequired(p => p.InventoryItem)
                .WithMany()
                .HasForeignKey(p => p.CogInventoryId);
        }
    }

}
