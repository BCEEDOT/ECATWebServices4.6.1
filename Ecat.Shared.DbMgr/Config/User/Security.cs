using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.User;

namespace Ecat.Shared.DbMgr.Config
{
    public class ConfigSecurity : EntityTypeConfiguration<Security>
    {
        public ConfigSecurity()
        {
            HasKey(p => p.PersonId);
            Property(p => p.PasswordHash).HasMaxLength(400);
            HasRequired(p => p.Person)
             .WithOptional(p => p.Security);
        }
    }
}
