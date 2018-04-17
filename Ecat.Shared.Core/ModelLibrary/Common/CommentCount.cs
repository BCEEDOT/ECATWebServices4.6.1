using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Common
{
    [TsIgnore]
    public class CommentCount
    {
        public int AuthorId { get; set; }
        public int NumOfComments { get; set; }
    }
}
