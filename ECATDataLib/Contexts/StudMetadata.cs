﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
//using Ecat.Shared.Core.Interface;
using Ecat.Data.Models.Interface;
//using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Data.Models.Common;
//using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Data.Models.Designer;
//using Ecat.Shared.Core.ModelLibrary.Faculty;
using Ecat.Data.Models.Faculty;
using Ecat.Data.Models.Faculty.Config;
//using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Data.Models.Student;
using Ecat.Data.Models.Student.Config;
//using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Data.Models.School;
using Ecat.Data.Models.School.Config;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
//using Ecat.Shared.DbMgr.Config;
//using Ecat.Shared.DbMgr.Context;
using Ecat.Data.Contexts.Config;

namespace Ecat.Data.Contexts
{
    public class StudMetadataCtx : ContextBase<StudMetadataCtx>
    {
        protected override void OnModelCreating(DbModelBuilder mb)
        {

            mb.Configurations.Add(new ConfigSpResponse());
            mb.Configurations.Add(new ConfigSpResult());
            mb.Configurations.Add(new ConfigStratResponse());
            mb.Configurations.Add(new ConfigStudentInCourse());
            mb.Configurations.Add(new ConfigStratResult());

            mb.Configurations.Add(new StudConfigSanitizedComment());
            mb.Configurations.Add(new StudConfigSanitizedResponse());
            mb.Configurations.Add(new StudConfigSpInstrument());
            mb.Configurations.Add(new StudConfigSpInventory());
            mb.Configurations.Add(new StudConfigStudWrkGrp());
            mb.Configurations.Add(new StudConfigCrse());
            mb.Configurations.Add(new StudConfigProfileStudent());
            mb.Configurations.Add(new StudConfigPerson());
            mb.Configurations.Add(new StudConfigCrseStudInWg());
            mb.Configurations.Add(new StudConfigSpComment());
            mb.Configurations.Add(new ConfigSpCommentFlag());
            mb.Configurations.Add(new ConfigFacSpResponse());
            mb.Configurations.Add(new ConfigFacultyInCourse());
            mb.Configurations.Add(new StudConfigProfileFaculty());

            mb.Ignore(new List<Type>
            {
                typeof (ProfileExternal),
                typeof (ProfileDesigner),
                //typeof (ProfileFaculty),
                typeof (Security),
                typeof (ProfileStaff),
                //typeof (FacultyInCourse),
                //typeof (FacSpResponse),
                typeof (FacStratResponse),
                typeof (FacSpComment),
                //typeof (KcResponse),
                //typeof (KcResult)
            });

            mb.Types()
                .Where(t => typeof(IAuditable).IsAssignableFrom(t))
                .Configure(p => p.Ignore("ModifiedById"));

            mb.Types()
                .Where(t => typeof(IAuditable).IsAssignableFrom(t))
                .Configure(p => p.Ignore("ModifiedDate"));

            //var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            //    .Where(type => !string.IsNullOrEmpty(type.Namespace))
            //    .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof (EntityTypeConfiguration<>));

            //foreach (var configurationInstance in typesToRegister.Select(Activator.CreateInstance))
            //{
            //    mb.Configurations.Add((dynamic)configurationInstance);
            //}

            mb.Ignore<CourseReconResult>();
            mb.Ignore<MemReconResult>();
            mb.Ignore<GroupMemReconResult>();
            mb.Ignore<GroupReconResult>();
            mb.Entity<FacultyInCourse>().Ignore(p => p.ReconResultId);
            mb.Entity<StudentInCourse>().Ignore(p => p.ReconResultId);
            mb.Entity<CrseStudentInGroup>().Ignore(p => p.ReconResultId);
            mb.Entity<WorkGroup>().Ignore(p => p.ReconResultId);
            mb.Entity<Course>().Ignore(p => p.ReconResultId);

            base.OnModelCreating(mb);
        }

        public IDbSet<WorkGroup> WorkGroups { get; set; }
        public IDbSet<Course> Courses { get; set; }
        public IDbSet<CrseStudentInGroup> StudentInGroups { get; set; }
        public IDbSet<StudentInCourse> StudentInCourses { get; set; }
        public IDbSet<SpResponse> SpResponses { get; set; }
        public IDbSet<SpResult> SpResults { get; set; }
        public IDbSet<StudSpComment> StudSpComments { get; set; }
        public IDbSet<StratResponse> StratRepoResponses { get; set; }
        public IDbSet<StratResult> StratResults { get; set; }
        public IDbSet<SpInventory> Inventories { get; set; } 
    }

}

