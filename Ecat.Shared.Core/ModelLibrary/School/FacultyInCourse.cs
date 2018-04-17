using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Shared.Core.ModelLibrary.Faculty;
using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Shared.Core.ModelLibrary.User;
using Newtonsoft.Json;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.School
{
    [TsClass(Module = "ecat.entity.s.school")]
    public class FacultyInCourse: ISoftDelete, ICompositeEntity
    {
        public string EntityId => $"{FacultyPersonId}, {CourseId}";
        public int CourseId { get; set; }
        public int FacultyPersonId  { get; set; }
        public string BbCourseMemId { get; set; }
        public Course Course { get; set; }
        public ProfileFaculty FacultyProfile { get; set; }
            
        public ICollection<FacSpResponse> FacSpResponses { get; set; }
        public ICollection<FacSpComment> FacSpComments { get; set; } 
        public ICollection<FacStratResponse> FacStratResponse { get; set; }
        public ICollection<StudSpCommentFlag> FlaggedSpComments { get; set; }

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
