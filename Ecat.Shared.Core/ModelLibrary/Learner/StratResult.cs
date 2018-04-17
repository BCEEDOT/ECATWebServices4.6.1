using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.Faculty;
using Ecat.Shared.Core.ModelLibrary.School;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Learner
{
    [TsClass(Module = "ecat.entity.s.learner")]
    public class StratResult : ICompositeEntity, IAuditable
    {
        public string EntityId => $"{StudentId}|{CourseId}|{WorkGroupId}";
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public int WorkGroupId { get; set; }

        public int OriginalStratPosition { get; set; }
        public int FinalStratPosition { get; set; }
        public decimal StratCummScore { get; set; }
        public decimal StudStratAwardedScore { get; set; }
        public decimal FacStratAwardedScore { get; set; }

        public Course Course { get; set; }
        public CrseStudentInGroup ResultFor { get; set; }
        public FacStratResponse FacStrat { get; set; }
        public ICollection<StratResponse> StratResponses { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
