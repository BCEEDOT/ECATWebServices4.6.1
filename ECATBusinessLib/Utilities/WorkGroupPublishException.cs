using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Business.Utilities
{
    class WorkGroupPublishException : Exception
    {
        public WorkGroupPublishException() { }

        public WorkGroupPublishException(string message) : base(message)
        {

        }

    }
}
