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
        internal static string _issuer;

        private readonly IOAuthAuthorizationServerProvider _authProvider;


        public AuthServerOptions(string issuer)
        {
            _issuer = issuer;
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
            Provider = _authProvider,
            AccessTokenFormat = new CustomJwtFormat(_issuer)

        };

    }

    public class AuthorizationServer : OAuthAuthorizationServerProvider
    {
        private IdToken _idToken;

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

            //identity.AddClaim(new Claim(ClaimTypes.Role, MpRoleTransform.InstituteRoleToEnum(person.MpInstituteRole).ToString()));

            switch (person.MpInstituteRole)
            {
                case MpInstituteRoleId.Student:
                    identity.AddClaim(new Claim(ClaimTypes.Role, MpInstituteRole.Student));
                    break;
                case MpInstituteRoleId.Faculty:
                    identity.AddClaim(new Claim(ClaimTypes.Role, MpInstituteRole.Faculty));
                    if (person.Faculty.IsCourseAdmin)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, MpInstituteRole.ISA));
                    }
                    else
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, MpInstituteRole.notISA));
                    }
                    break;
                default:
                    identity.AddClaim(new Claim(ClaimTypes.Role, MpInstituteRoleId.Student));
                    break;
            }


            _idToken = new IdToken
            {
                TokenExpire = DateTime.Now.Add(TimeSpan.FromHours(24)),
                TokenExpireWarning = DateTime.Now.Add(TimeSpan.FromHours(23)),
                lastName = person.LastName,
                firstName = person.FirstName,
                email = person.Email,
                mpAffiliation = person.MpAffiliation,
                mpComponent = person.MpComponent,
                mpPaygrade = person.MpPaygrade,
                mpGender = person.MpGender,
                mpInstituteRole = person.MpInstituteRole,
                registrationComplete = person.RegistrationComplete,
                PersonId = person.PersonId
            };

            var ticket = new AuthenticationTicket(identity, null);

            oauthCtx.Validated(ticket);

            await Task.FromResult(oauthCtx.Validated());

        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            var tokenString = JsonConvert.SerializeObject(_idToken, Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                }
                );
            context.AdditionalResponseParameters.Add("id_token", tokenString);
            return Task.FromResult(true);

        }
    }
}
