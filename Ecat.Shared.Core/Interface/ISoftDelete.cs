using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Shared.Core.Interface
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        int? DeletedById { get; set; }
        DateTime? DeletedDate { get; set; }
    }
}
