using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Data.Models.Common;
//using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Data.Models.Student;
//using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Data.Models.School;
//using Ecat.Shared.DbMgr.Context;
using Ecat.Data.Models.User.Config;
using Ecat.Data.Models.School.Config;
using Ecat.Data.Models.Faculty.Config;
using Ecat.Data.Models.Student.Config;
using Ecat.Data.Models.Faculty;
using Ecat.Data.Models.User;

namespace Ecat.Data.Contexts
{
    public class LmsAdminMetadataCtx : EcatContext
    {
        protected override void OnModelCreating(DbModelBuilder mb)
        {
            //Database.Log = s => Debug.WriteLine(s);

            //mb.Conventions.Remove<PluralizingTableNameConvention>();

            //mb.Properties<string>().Configure(s => s.HasMaxLength(50));

            //mb.Properties()
            //    .Where(p => p.Name.StartsWith("Mp") || p.Name.StartsWith("En"))
            //    .Configure(x => x.HasColumnName(x.ClrPropertyInfo.Name.Substring(2)));

            //mb.Types()
            //    .Where(type => type.Name.StartsWith("Ec"))
            //    .Configure(type => type.ToTable(type.ClrType.Name.Substring(2)));

            //var typesToRegister = Assembly.GetAssembly(typeof (EcatContext)).GetTypes()
            //    .Where(type => type.IsClass && type.Namespace == "Ecat.Shared.DbMgr.Config");

            //foreach (var configurationInstance in typesToRegister.Select(Activator.CreateInstance))
            //{
            //    mb.Configurations.Add((dynamic) configurationInstance);
            //}

            //mb.Properties<DateTime>()
            //    .Configure(c => c.HasColumnType("datetime2"));

            //mb.Ignore<Academy>();
            //mb.Ignore<AcademyCategory>();
            //mb.Ignore<SanitizedSpComment>();
            //mb.Ignore<SanitizedSpResponse>();

            mb.Configurations.Add(new ConfigProfileStudent());
            mb.Configurations.Add(new ConfigProfileFaculty());
            mb.Configurations.Add(new ConfigSpResult());
            mb.Configurations.Add(new ConfigStratResult());
            mb.Configurations.Add(new ConfigCrseStudInGroup());
            mb.Configurations.Add(new ConfigFacultyInCourse());
            mb.Configurations.Add(new ConfigStudentInCourse());
            mb.Configurations.Add(new ConfigFacStratResponse());
            mb.Configurations.Add(new ConfigStratResponse());
            mb.Configurations.Add(new ConfigPerson());
            mb.Configurations.Add(new ConfigFacSpResponse());

            //mb.Ignore<Academy>();
            mb.Ignore<Security>();
            mb.Ignore<SanitizedSpComment>();
            mb.Ignore<SanitizedSpResponse>();
            mb.Ignore<SpResponse>();
            //mb.Ignore<FacSpResponse>();
            //mb.Ignore<SpResult>();
            //mb.Ignore<StratResponse>();
            //mb.Ignore<FacStratResponse>();
            mb.Ignore<StudSpComment>();
            mb.Ignore<StudSpCommentFlag>();
            mb.Ignore<FacSpCommentFlag>();
            mb.Ignore<FacSpComment>();
            mb.Ignore<RoadRunner>();

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
