using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Business.Utilities
{
    public class UserUpdateException : Exception {
        public UserUpdateException() { }
        public UserUpdateException(string message) : base(message) { }
    }

    public class InvalidEmailException: Exception
    {
        public InvalidEmailException() { }
        public InvalidEmailException(string message) : base(message) { }
    }

    public class CanvasAccessTokenMissingException : Exception
    {
        public CanvasAccessTokenMissingException() { }

        public CanvasAccessTokenMissingException(string message) : base(message)
        {

        }
    }

    public class MemberMissingPublishDataException : Exception
    {

        public MemberMissingPublishDataException() { }

        public MemberMissingPublishDataException(string message) : base(message)
        {

        }
    }

    public class WorkGroupPublishException : Exception
    {
        public WorkGroupPublishException() { }

        public WorkGroupPublishException(string message) : base(message)
        {

        }

    }
}
