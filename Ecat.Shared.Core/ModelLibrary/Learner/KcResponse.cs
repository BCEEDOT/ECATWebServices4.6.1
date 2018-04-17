using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Shared.Core.ModelLibrary.School;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Learner
{
    [TsClass(Module = "ecat.entity.s.learner")]
    public class KcResponse: ICompositeEntity
    {
        public string EntityId => $"{StudentId}|{CourseId},{InventoryId},{Version}";
        public int InventoryId { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }

        public int? ResultId { get; set; }
        public bool IsCorrect { get; set; }
        public int Version { get; set; }
        public bool AllowNewAttempt { get; set; }

        public StudentInCourse Student { get; set; }
        public KcResult Result { get; set; }
        public KcInventory Inventory { get; set; }
    }
}
