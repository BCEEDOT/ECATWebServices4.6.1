using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Business.Utilities
{
    class MemberMissingPublishDataException : Exception
    {

        public MemberMissingPublishDataException() { }

        public MemberMissingPublishDataException(string message) : base(message)
        {

        }
    }
}
