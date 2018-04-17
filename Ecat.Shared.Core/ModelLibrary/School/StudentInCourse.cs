using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Shared.Core.ModelLibrary.User;
using Newtonsoft.Json;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.School
{
    [TsClass(Module = "ecat.entity.s.school")]
    public class StudentInCourse : ISoftDelete, ICompositeEntity
    {
        public string EntityId => $"{StudentPersonId}|{CourseId}";
        public int CourseId { get; set; }
        public int StudentPersonId  { get; set; }
        public string BbCourseMemId { get; set; }

        public Course Course { get; set; }
        public ProfileStudent Student { get; set; }

        public ICollection<CrseStudentInGroup> WorkGroupEnrollments { get; set; }
        public ICollection<KcResponse> KcResponses { get; set; }
        [JsonIgnore][TsIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore][TsIgnore]
        public int? DeletedById { get; set; }

        [JsonIgnore][TsIgnore]
        public DateTime? DeletedDate { get; set; }

        public Guid? ReconResultId { get; set; }
        public MemReconResult ReconResult { get; set; }

    }
}
