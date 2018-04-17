using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ecat.Shared.Core.Utility;
using Ecat.Web.Utility;

namespace Ecat.Web.Controllers
{
    [EcatRolesAuthorized(Is = new[] { RoleMap.HqStaff })]
    public class StaffController : EcatBaseBreezeController
    {
        public StaffController()
        {
            
        }
    }
}
