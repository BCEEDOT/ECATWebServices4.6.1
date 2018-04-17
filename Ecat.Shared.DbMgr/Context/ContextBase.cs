using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Shared.Core.ModelLibrary.User;

namespace Ecat.Shared.DbMgr.Context
{
    public class ContextBase<TContext> : DbContext where TContext : DbContext
    {
        static ContextBase()
        {
            Database.SetInitializer<TContext>(null);
        }

        protected ContextBase() : base("ecat")
        {
            Database.Log = s => Debug.WriteLine(s);

            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            mb.Conventions.Remove<PluralizingTableNameConvention>();

            mb.Properties<string>().Configure(s => s.HasMaxLength(50));

            mb.Properties()
                .Where(p => p.Name.StartsWith("Mp") || p.Name.StartsWith("En"))
                .Configure(x => x.HasColumnName(x.ClrPropertyInfo.Name.Substring(2)));

            mb.Types()
                .Where(type => type.Name.StartsWith("Ec"))
                .Configure(type => type.ToTable(type.ClrType.Name.Substring(2)));

            mb.Types()
                 .Where(t => typeof(ISoftDelete).IsAssignableFrom(t))
                 .Configure(p => p.Ignore("DeletedById"));

            mb.Types()
                .Where(t => typeof(ISoftDelete).IsAssignableFrom(t))
                .Configure(p => p.Ignore("DeletedDate"));

            mb.Ignore<Academy>();

            mb.Ignore<AcademyCategory>();

            mb.Properties<DateTime>()
                .Configure(c => c.HasColumnType("datetime2"));

        }
    }
}
