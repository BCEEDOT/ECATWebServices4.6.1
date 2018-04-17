using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider.EF6;
using Ecat.DesignerMod.Core.Data;
using Ecat.DesignerMod.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.User;

namespace Ecat.DesignerMod.Core.Business
{
    public class DesignerLogic : IDesignerLogic
    {
        private readonly EFContextProvider<DesignerCtx> _efCtx;
        public ProfileDesigner Designer { get; set; }
        
        public DesignerLogic(DesignerCtx ctx, EFContextProvider<DesignerCtx> efCtx )
        {
            _efCtx = efCtx;
        }

        public string Metadata => _efCtx.Metadata();



    }
}
