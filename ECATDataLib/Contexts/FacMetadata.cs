﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
//using Ecat.Shared.Core.Interface;
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
using Ecat.Data.Models.Canvas.Config;

namespace Ecat.Data.Contexts
{
    public class FacMetadataCtx : ContextBase<FacMetadataCtx>
    {
        protected override void OnModelCreating(DbModelBuilder mb)
        {

            mb.Configurations.Add(new ConfigSpResponse());
            mb.Configurations.Add(new ConfigSpResult());
            mb.Configurations.Add(new ConfigStratResponse());
            mb.Configurations.Add(new ConfigStratResult());
            mb.Configurations.Add(new ConfigSpComment());
            mb.Configurations.Add(new ConfigFacSpCommentFlag());
            mb.Configurations.Add(new ConfigSpCommentFlag());
            mb.Configurations.Add(new ConfigFacSpResponse());
            mb.Configurations.Add(new ConfigFacStratResponse());
            mb.Configurations.Add(new ConfigFacSpComment());
            mb.Configurations.Add(new ConfigCrseStudInGroup());
            mb.Configurations.Add(new ConfigFacultyInCourse());
            mb.Configurations.Add(new ConfigStudentInCourse());

            mb.Configurations.Add(new FacConfigStudSpInstrument());
            mb.Configurations.Add(new FacConfigStudSpInventory());
            mb.Configurations.Add(new FacConfigProfileStudent());
            mb.Configurations.Add(new FacConfigProfileFaculty());
            mb.Configurations.Add(new FacConfigPerson());
            mb.Configurations.Add(new FacConfigStudWrkGrp());
            mb.Configurations.Add(new FacConfigStudCrse());
            mb.Configurations.Add(new ConfigCanvasLogin());


            //var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            //    .Where(type => !string.IsNullOrEmpty(type.Namespace))
            //    .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));

            //foreach (var configurationInstance in typesToRegister.Select(Activator.CreateInstance))
            //{
            //    mb.Configurations.Add((dynamic)configurationInstance);
            //}

            mb.Ignore(new List<Type>
            {
                typeof (ProfileExternal),
                typeof (ProfileDesigner),
                typeof (Security),
                typeof (ProfileStaff),
                //typeof (KcResponse),
                //typeof (KcResult),
                typeof (SanitizedSpComment),
                typeof (SanitizedSpResponse)
            });

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
        public IDbSet<SpInstrument> SpInstruments { get; set; } 
        public IDbSet<CrseStudentInGroup> StudentInGroups { get; set; }
        public IDbSet<StudentInCourse> StudentInCourses { get; set; }
        public IDbSet<FacultyInCourse> FacultyInCourses { get; set; }
        public IDbSet<FacSpResponse> FacSpResponses { get; set; }
        public IDbSet<SpResponse> SpResponses { get; set; }
        public IDbSet<FacStratResponse> FacStratResponses { get; set; }
        public IDbSet<StratResponse> StratResponses { get; set; }
        public IDbSet<FacSpComment> FacSpComments { get; set; }
        public IDbSet<StudSpComment> StudSpComments { get; set; }
        public IDbSet<SpResult> SpResults { get; set; }
        public IDbSet<StratResult> SpStratResults { get; set; }
        public IDbSet<CourseReconResult> CourseRecon { get; set; }
        public IDbSet<MemReconResult> MemRecon { get; set; }
        public IDbSet<GroupReconResult> GroupRecon { get; set; }
        public IDbSet<GroupMemReconResult> GroupMemRecon { get; set; }
    }

}
