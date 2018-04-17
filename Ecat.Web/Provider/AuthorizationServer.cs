using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.Core.Provider;
using Ecat.Shared.Core.Utility;
using Ecat.UserMod.Core;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ecat.Web.Provider
{
    public static class UserAuthToken
    {
        public static ClaimsIdentity GetClaimId => new ClaimsIdentity(AuthServerOptions.OabOpts.AuthenticationType);
    }

    public class AuthServerOptions
    {
        internal static OAuthBearerAuthenticationOptions OabOpts { get; set; }

        private readonly IOAuthAuthorizationServerProvider _authProvider;

        public AuthServerOptions()
        {
            _authProvider = new AuthorizationServer();
        }

        public OAuthAuthorizationServerOptions OauthOpts => new OAuthAuthorizationServerOptions
        {
            //TODO: change to false for deployment
            AllowInsecureHttp = true,
            AuthenticationType = OAuthDefaults.AuthenticationType,
            AuthenticationMode = AuthenticationMode.Active,
            TokenEndpointPath = new PathString("/connect/token"),
            AccessTokenExpireTimeSpan = TimeSpan.FromHours(24),
            Provider = _authProvider
        };

    }

    public class AuthorizationServer : OAuthAuthorizationServerProvider
    {
        private LoginToken _loginToken;

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext ctx)
        {
            return Task.FromResult(ctx.Validated());
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext oauthCtx)
        {
            oauthCtx.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            if (oauthCtx.UserName == null || oauthCtx.Password == null)
            {
                oauthCtx.SetError("invalid_grant", "Null credentials were preseted");
                return;
            }

            Person person;

            using (var dbCtx = new UserCtx())
            {
                //TODO: Need to fix throws 500 error if username not found.
                person = await dbCtx.People.Include(user => user.Security).SingleAsync(user => user.Email == oauthCtx.UserName);
            }

            var hasValidPassword = PasswordHash.ValidatePassword(oauthCtx.Password, person.Security.PasswordHash);

            if (!hasValidPassword)
            {
                 
                throw new UnauthorizedAccessException("Invalid Username/Password Combination");
            }

            var identity = UserAuthToken.GetClaimId;

            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, person.PersonId.ToString()));

            identity.AddClaim(new Claim(ClaimTypes.Role, MpRoleTransform.InstituteRoleToEnum(person.MpInstituteRole).ToString()));

            _loginToken = new LoginToken
            {
                TokenExpire = DateTime.Now.Add(TimeSpan.FromHours(24)),
                TokenExpireWarning = DateTime.Now.Add(TimeSpan.FromHours(23)),
                Person = person,
                PersonId = person.PersonId
            };

            oauthCtx.Validated(identity);

            await Task.FromResult(oauthCtx.Validated());

        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            var tokenString = JsonConvert.SerializeObject(_loginToken, Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                }
                );
            context.AdditionalResponseParameters.Add("loginToken", tokenString);
            return Task.FromResult(true);

        }
    }
}
