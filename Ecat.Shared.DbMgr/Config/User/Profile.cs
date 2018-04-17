using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.User;

namespace Ecat.Shared.DbMgr.Config
{
    public class ConfigProfileStudent : EntityTypeConfiguration<ProfileStudent>
    {
        public ConfigProfileStudent()
        {
            HasKey(p => p.PersonId);

            Property(p => p.Bio)
                .HasMaxLength(6000);
        }
    }

    public class ConfigProfileFaculty : EntityTypeConfiguration<ProfileFaculty>
    {
        public ConfigProfileFaculty()
        {
            HasKey(p => p.PersonId);

            Property(p => p.Bio)
                .HasMaxLength(6000);
        }
    }

    public class ConfigProfileDesigner : EntityTypeConfiguration<ProfileDesigner>
    {
        public ConfigProfileDesigner()
        {
            HasKey(p => p.PersonId);

            Property(p => p.Bio)
                .HasMaxLength(6000);
        }
    }

    public class ConfigProfileStaff : EntityTypeConfiguration<ProfileStaff>
    {
        public ConfigProfileStaff()
        {
            HasKey(p => p.PersonId);

            Property(p => p.Bio)
                .HasMaxLength(6000);
        }
    }

    public class ConfigProfileExternal : EntityTypeConfiguration<ProfileExternal>
    {
        public ConfigProfileExternal()
        {
            HasKey(p => p.PersonId);

            Property(p => p.Bio)
                .HasMaxLength(6000);
        }
    }
}
