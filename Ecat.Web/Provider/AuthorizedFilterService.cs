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
using System.Web.Http.Filters;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.Core.Utility;
using Ecat.UserMod.Core;
using Ecat.Web.Controllers;

namespace Ecat.Web.Provider
{
    public class AuthorizedFilterService : IAuthenticationFilter
    {
        public bool AllowMultiple { get; }

        private readonly RoleMap[] _authRoles;

        public AuthorizedFilterService(RoleMap[] roles)
        {
            _authRoles = roles;
        }

        private static bool SkipAuthorization(HttpAuthenticationContext httpContext)
        {
            Contract.Assert(httpContext != null);

            return httpContext.ActionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || httpContext.ActionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext httpContext, CancellationToken token)
        {
            #region Check if userId is in the claims

            var principal = httpContext.Principal as ClaimsPrincipal;

            if (principal == null || !principal.Identity.IsAuthenticated)
            {
                if (!SkipAuthorization(httpContext)) throw new Exception("ECAT Error: Unable to detect principal users");
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
                httpContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
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

            if (_authRoles != null && !_authRoles.Contains(roleClaim))
            {
                httpContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized access");
            }

            #endregion

            Person user;
            using (var ctx = new UserCtx())
            {
                user = await((DbSet<Person>)ctx.People).FindAsync(token, parsedUid);
            }

            if (user == null)
            {
                httpContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Unable to locate user from Authorization Filter using data store");
            }

            var controller = httpContext.ActionContext.ControllerContext.Controller as EcatBaseBreezeController;

            Contract.Assert(controller != null);

            controller.SetUser(user);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}