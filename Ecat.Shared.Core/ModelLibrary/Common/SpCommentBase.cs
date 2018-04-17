using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Shared.Core.ModelLibrary.School;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Common
{
    [TsClass(Module = "ecat.entity.s.common")]
    public class SpCommentBase: IAuditable 
    {
        public DateTime CreatedDate { get; set; }
        public bool Anonymity { get; set; }
        public string CommentText { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
