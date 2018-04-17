using Breeze.ContextProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Shared.Core.Utility
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SaveGuardAttribute : Attribute { }

    public class UserGuardAttribute : SaveGuardAttribute { };

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DeletableGuardAttribute : Attribute
    {
        public RoleMap[] AuthorizedDeleters { get; set; }
    }

    public static class EcatExtensions
    {
        
    }
    
}
