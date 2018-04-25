﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Ecat.Shared.Core.Interface;
using Ecat.Data.Models.Interface;
//using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Data.Models.School;
using TypeLite;

namespace Ecat.Data.Models.Student
{
    [TsClass(Module = "ecat.entity.s.learner")]
    public class StratResponse : IAuditable, ICompositeEntity, IWorkGroupMonitored, ICourseMonitored
    {
        public string EntityId => $"{AssessorPersonId}|{AssesseePersonId}|{CourseId}|{WorkGroupId}";
        public int AssessorPersonId { get; set; }
        public int AssesseePersonId { get; set; }
        public int CourseId { get; set; }
        public int WorkGroupId { get; set; }
        public int StratPosition { get; set; }

        public CrseStudentInGroup Assessor { get; set; }
        public CrseStudentInGroup Assessee { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
