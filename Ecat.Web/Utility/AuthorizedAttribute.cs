using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.Core.Utility;
using Ecat.UserMod.Core;
using Ecat.Web.Controllers;
using AllowAnonymousAttribute = System.Web.Http.AllowAnonymousAttribute;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;

namespace Ecat.Web.Utility
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EcatRolesAuthorized : AuthorizeAttribute
    {
        public RoleMap[] Is { get; set; }

        public override bool AllowMultiple => false;

        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {

           await base.OnAuthorizationAsync(actionContext, cancellationToken);

            #region Check if userId is in the claims

            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (principal == null || !principal.Identity.IsAuthenticated)
            {
                if (!SkipAuthorization(actionContext)) throw new HttpResponseException(HttpStatusCode.Unauthorized);
                await Task.FromResult<object>(null);
                return;
            }

            var parsedUid = 0;
            var stringUid = principal.FindFirst(ClaimTypes.PrimarySid).Value;
            if (stringUid != null)
            {
                int.TryParse(stringUid, out parsedUid);
            }

            if (parsedUid == 0)
            {
                actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Unable to locate user from Authorization Filter");
            }

            #endregion

            #region Check if known roles is present

            var roleClaim = RoleMap.Unknown;

            var stringRoleClaim = principal.FindFirst(ClaimTypes.Role).Value;
            if (stringRoleClaim != null)
            {
                Enum.TryParse(stringRoleClaim, out roleClaim);
            }

            if (Is != null && !Is.Contains(roleClaim))
            {
                actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized access");
            }

            #endregion

            Person user;

            using (var ctx = new UserCtx())
            {
                user = await ((DbSet<Person>)ctx.People).FindAsync(cancellationToken, parsedUid);

                if (Is != null && Is.Contains(RoleMap.CrseAdmin))
                {
                    await ctx.Entry(user).Reference(u => u.Faculty).LoadAsync(cancellationToken);
                    if (user.Faculty == null || !user.Faculty.IsCourseAdmin)
                    {
                        actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized access");

                    }
                }
            }

            if (user == null)
            {
                actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Unable to locate user from Authorization Filter using data store");
            }

            var controller = actionContext.ControllerContext.Controller as EcatBaseBreezeController;

            Contract.Assert(controller != null);

            controller.SetUser(user);
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            Contract.Assert(actionContext != null);

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

       

    }
}