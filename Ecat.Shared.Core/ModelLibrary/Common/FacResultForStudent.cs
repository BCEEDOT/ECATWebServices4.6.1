using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.Faculty;

namespace Ecat.Shared.Core.ModelLibrary.Common
{
    public class FacResultForStudent
    {
        public int StudentId  { get; set; }
        public ICollection<FacSpResponse> FacResponses   { get; set; }
        public FacSpComment FacSpComment { get; set; }
        public FacSpCommentFlag FacSpCommentFlag { get; set; } 
    }
}
