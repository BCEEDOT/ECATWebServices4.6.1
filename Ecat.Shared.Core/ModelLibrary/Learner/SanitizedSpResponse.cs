using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.Designer;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Learner
{
    [TsClass(Module = "ecat.entity.s.learner")]
    public class SanitizedSpResponse
    {
        public Guid Id { get; set; }
        public int CourseId { get; set; }
        //public int StudentId { get; set; }
        public int AssesseeId { get; set; }
        public int WorkGroupId { get; set; }
        public bool IsSelfResponse { get; set; }
        public string PeerGenericName { get; set; }
        public string MpItemResponse { get; set; }
        public int ItemModelScore { get; set; }
        public int InventoryItemId { get; set; }
        public SpInventory InventoryItem { get; set; }
        public SpResult Result { get; set; }
    }
}
