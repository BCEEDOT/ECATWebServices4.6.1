using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
//using System.Web.Http;
using System.Web.Mvc;
//using Ecat.Shared.Core.ModelLibrary.Common;
//using Ecat.Shared.Core.ModelLibrary.User;
//using Ecat.Shared.Core.Utility;
//using Ecat.Shared.DbMgr.Context;
//using Ecat.UserMod.Core;
using Ecat.Business.Repositories.Interface;


namespace Ecat.Web.Controllers
{
    public class CanvasAuthController : Controller
    {
        private readonly ILmsAdminTokenRepo tokenRepo;

        public CanvasAuthController(ILmsAdminTokenRepo lmsTokenRepo)
        {
            tokenRepo = lmsTokenRepo;
        }

        [HttpGet]
        public async Task<ActionResult> TokenRequest(string code)
        {
            var token = await tokenRepo.GetRefreshToken(code);
            if (token)
            {
                ViewBag.Success = true;
            }
            else
            {
                ViewBag.Success = false;
            }

            return View();
        }
    }
}


