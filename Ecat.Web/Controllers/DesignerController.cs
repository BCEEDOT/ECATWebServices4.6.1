using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ecat.DesignerMod.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.Core.Utility;
using Ecat.Web.Utility;

namespace Ecat.Web.Controllers
{
    [EcatRolesAuthorized(Is = new[] { RoleMap.Designer })]
    public class DesignerController : EcatBaseBreezeController
    {
        private readonly IDesignerLogic _designLogic; 

        public DesignerController(IDesignerLogic designLogic)
        {
            _designLogic = designLogic;
        }

        internal override void SetUser(Person person)
        {
            _designLogic.Designer = person.Designer;
        }

        [HttpGet]
        public string Metadata()
        {
            return _designLogic.Metadata;
        }
    }
}
