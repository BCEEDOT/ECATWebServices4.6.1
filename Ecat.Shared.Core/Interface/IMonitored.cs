using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Shared.Core.Interface
{
    public interface ICourseMonitored
    {
        int CourseId { get; set; }
    }

    public interface IWorkGroupMonitored
    {
        int WorkGroupId { get; set; }
    }
}
