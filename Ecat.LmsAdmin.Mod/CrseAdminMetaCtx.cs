using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Shared.DbMgr.Context;

namespace Ecat.LmsAdmin.Mod
{
    public class CrseAdminMetaCtx : EcatContext
    {
        protected override void OnModelCreating(DbModelBuilder mb)
        {
            Database.Log = s => Debug.WriteLine(s);

            mb.Conventions.Remove<PluralizingTableNameConvention>();

            mb.Properties<string>().Configure(s => s.HasMaxLength(50));

            mb.Properties()
                .Where(p => p.Name.StartsWith("Mp") || p.Name.StartsWith("En"))
                .Configure(x => x.HasColumnName(x.ClrPropertyInfo.Name.Substring(2)));

            mb.Types()
                .Where(type => type.Name.StartsWith("Ec"))
                .Configure(type => type.ToTable(type.ClrType.Name.Substring(2)));

            var typesToRegister = Assembly.GetAssembly(typeof (EcatContext)).GetTypes()
                .Where(type => type.IsClass && type.Namespace == "Ecat.Shared.DbMgr.Config");

            foreach (var configurationInstance in typesToRegister.Select(Activator.CreateInstance))
            {
                mb.Configurations.Add((dynamic) configurationInstance);
            }

            mb.Properties<DateTime>()
                .Configure(c => c.HasColumnType("datetime2"));

            mb.Ignore<Academy>();
            mb.Ignore<AcademyCategory>();
            mb.Ignore<SanitizedSpComment>();
            mb.Ignore<SanitizedSpResponse>();

            mb.Entity<WorkGroup>()
                .HasOptional(p => p.ReconResult)
                .WithMany(p => p.Groups)
                .HasForeignKey(p => p.ReconResultId);

            mb.Entity<Course>()
                .HasOptional(p => p.ReconResult)
                .WithMany(p => p.Courses)
                .HasForeignKey(p => p.ReconResultId);

            mb.Entity<CrseStudentInGroup>()
                .HasOptional(p => p.ReconResult)
                .WithMany(p => p.GroupMembers)
                .HasForeignKey(p => p.ReconResultId);

            mb.Entity<FacultyInCourse>()
                .HasOptional(p => p.ReconResult)
                .WithMany(p => p.Faculty)
                .HasForeignKey(p => p.ReconResultId);

            mb.Entity<StudentInCourse>()
                .HasOptional(p => p.ReconResult)
                .WithMany(p => p.Students)
                .HasForeignKey(p => p.ReconResultId);
        }
    }
}
