﻿using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
//using Ecat.Shared.Core.ModelLibrary.Common;
//using Ecat.Shared.Core.ModelLibrary.User;
//using Ecat.Shared.Core.Utility;
//using Ecat.Shared.DbMgr.Context;
//using Ecat.UserMod.Core;
using Ecat.Business.Repositories.Interface;
using Ecat.Business.Utilities;
using Ecat.Data.Models.User;
using Ecat.Data.Static;
using Ecat.Data.Models.Common;
using Ecat.Web.Provider;
using LtiLibrary.AspNet.Extensions;
using LtiLibrary.Core.Lti1;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Configuration;

namespace Ecat.Web.Controllers
{
    public class LtiController : Controller
    {
        //private readonly IUserLogic _userLogic;
        private readonly IUserRepo _userRepo;

        public LtiController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
     
        // GET: Lti
        public async Task<ActionResult> LtiEntry()
        {
            var isLti = Request.IsAuthenticatedWithLti();
            if (!isLti)
            {
                return null;
            }

            var ltiRequest = new LtiRequest();

            ltiRequest.ParseRequest(Request);

            var person = new Person();

            try
            {
                person = await _userRepo.ProcessLtiUser(ltiRequest);
            }
            catch (InvalidEmailException ex)
            {
                ViewBag.Error = ex.Message + "\n\n Please update your email address in both the LMS and AU Portal to use ECAT.";
                return View();
            }
            catch (UserUpdateException) {
                ViewBag.Error = "There was an error updating your account with the information from the LMS. Please try again.";
                return View();
            }
            
            var token = new IdToken
            {
                TokenExpire = DateTime.Now.Add(TimeSpan.FromHours(24)),
                TokenExpireWarning = DateTime.Now.Add(TimeSpan.FromHours(23)),
                LastName = person.LastName,
                FirstName = person.FirstName,
                Email = person.Email,
                MpAffiliation = person.MpAffiliation,
                MpComponent = person.MpComponent,
                MpPaygrade = person.MpPaygrade,
                MpGender = person.MpGender,
                MpInstituteRole = person.MpInstituteRole,
                RegistrationComplete = person.RegistrationComplete,
                PersonId = person.PersonId
            };

            var identity = UserAuthToken.GetClaimId;
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, token.PersonId.ToString()));

            switch (person.MpInstituteRole)
            {
                case MpInstituteRoleId.Faculty:
                    //person.Faculty = null;
                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleMap.Faculty.ToString()));
                    if (person.Faculty.IsCourseAdmin)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, MpInstituteRole.ISA));
                    }
                    else
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, MpInstituteRole.notISA));
                    }
                    break;
                case MpInstituteRoleId.Student:
                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleMap.Student.ToString()));
                    break;
                default:
                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleMap.External.ToString()));
                    break;
            }

            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());

            ticket.Properties.IssuedUtc = DateTime.Now;
            ticket.Properties.ExpiresUtc = DateTime.Now.AddHours(24);

            //token.AuthToken = AuthServerOptions.OabOpts.AccessTokenFormat.Protect(ticket);
            var format = new CustomJwtFormat(ConfigurationManager.AppSettings["issuer"]);
            token.AuthToken = format.Protect(ticket);

            ViewBag.User = JsonConvert.SerializeObject(token, Formatting.None,
                 new JsonSerializerSettings
                 {
                     ContractResolver = new CamelCasePropertyNamesContractResolver(),
                     ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                 });

            return View();
        }

        public async Task<ActionResult> Ping(string site)
        {
            var usehttps = false;

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var cl = new HttpClient())
            {
                switch (site)
                {
                    case "sgoogle":
                        site = "www.google.com:443";
                        usehttps = true;
                        break;
                    case "smyaf":
                        site = "214.48.100.34:443";
                        usehttps = true;
                        break;
                    case "sbb":
                        site = "barnescenter.blackboard.com:443/webapps/ws/services/Context.WS?wsdl";
                        usehttps = true;
                        break;
                    case "soldecat":
                        site = "barnescenter-assessment1.azurewebsites.net:443/breeze/user/ping";
                        usehttps = true;
                        break;
                    case "scallhome":
                        site = "68.113.109.232:36064/ecat/lti/ping/catch";
                        usehttps = true;
                        break;
                    case "callhome":
                        site = "68.113.109.232:36064/ecat/lti/ping/catch";
                        usehttps = true;
                        break;
                    case "oldecat":
                        site = "barnescenter-assessment1.azurewebsites.net/breeze/user/ping";
                        break;
                    case "google":
                        site = "www.google.com";
                        break;
                    case "myaf":
                        site = "214.48.100.34";
                        break;
                    case "maxwell":
                        site = "www.airuniversity.af.mil";
                        break;
                    default:
                        site = "barnescenter.blackboard.com/webapps/ws/services/Context.WS?wsdl";
                        break;
                }

                var uri = new Uri($"http://{site}");
                if (usehttps) { uri = new Uri($"https://{site}");}
               

                //var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
                //{
                //    Version = HttpVersion.Version10
                //};

                var response = await cl.GetAsync(uri);

                ViewBag.ServerResponse = await response.Content.ReadAsStringAsync();
            }
            return View();
        }
    }
}


