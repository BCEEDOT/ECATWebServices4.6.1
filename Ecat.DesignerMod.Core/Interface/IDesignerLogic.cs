using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.User;

namespace Ecat.DesignerMod.Core.Interface
{
    public interface IDesignerLogic
    {
        ProfileDesigner Designer { get; set; }
        string Metadata { get; }
    }
}
