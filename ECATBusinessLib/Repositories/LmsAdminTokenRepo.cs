﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Contexts;

namespace Ecat.Business.Repositories
{
    public class TokenUser
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public TokenUser user { get; set; }
    }

    public class TokenOps : ILmsAdminTokenRepo
    {
        //private readonly EFPersistenceManager<EcatContext> ctxManager;
        private readonly EcatContext ecatContext;
        //TODO: Update once we have production Canvas
        private readonly string canvasTokenUrl = "https://lms.stag.af.edu/login/oauth2/token";
        private readonly string clientId = "10000000000002";
        private readonly string clientSecret = "yzoxRnkSeFZP07jmEyKgJb8QUC7SDmsMbdRKGjoABIYmha5LkTOlePg9hlVKFEmt";
        private readonly string redirectUri = "https://augateway.maxwell.af.mil";

        public int loggedInUserId { get; set; }

        public TokenOps(EcatContext mainCtx)
        {
            //ctxManager = new EFPersistenceManager<EcatContext>(mainCtx);
            ecatContext = mainCtx;
        }

        //return true is we have a refresh token for this user that works
        //return false should redirect the user to the canvas auth endpoint to get a code
        public async Task<bool> CheckCanvasTokenInfo()
        {
            var canvLogin = await ecatContext.CanvasLogins.Where(cl => cl.PersonId == loggedInUserId).SingleOrDefaultAsync();

            if (canvLogin == null)
            {
                return false;
            }

            if (canvLogin.RefreshToken == null)
            {
                return false;
            }

            var accessToken = await GetAccessToken();
            if (accessToken == null)
            {
                return false;
            }

            return true;
        }

        //after getting the code from the canvas auth endpoint come here
        public async Task<bool> GetRefreshToken(string authCode)
        {
            if (authCode == null)
            {
                return false;
            }

            var client = new HttpClient();

            var authAddr = new Uri(canvasTokenUrl);
            var content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("code", authCode)
                });

            var response = await client.PostAsync(authAddr, content);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var respContent = await response.Content.ReadAsStringAsync();
            var respObject = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(respContent);

            var ecatUser = await ecatContext.People.Where(p => p.BbUserId == respObject.user.id.ToString()).SingleAsync();

            if (ecatUser == null)
            {
                return false;
            }

            var canvLogin = await ecatContext.CanvasLogins
                .Where(cl => cl.PersonId == ecatUser.PersonId)
                .SingleOrDefaultAsync();

            if (respObject.access_token == null)
            {
                //we got a success code from the canvas call, but didn't get a token. can this happen?
                return false;
            }

            canvLogin.AccessToken = respObject.access_token;
            canvLogin.TokenExpires = DateTime.Now.AddSeconds(respObject.expires_in);

            //user can tell canvas to remember their authorization and give us a refresh token or not and we only get an access token
            if (respObject.refresh_token != null)
            {
                canvLogin.RefreshToken = respObject.refresh_token;
            }

            //Need a new login entry
            if (canvLogin.PersonId == 0)
            {
                canvLogin.PersonId = ecatUser.PersonId;
                ecatContext.CanvasLogins.Add(canvLogin);
            }
            else
            {
                ecatContext.CanvasLogins.Attach(canvLogin);
                ecatContext.Entry(canvLogin).State = System.Data.Entity.EntityState.Modified;
            }

            await ecatContext.SaveChangesAsync();

            return true;
        }

        //if this returns null, redirect user to the canvas auth endpoint
        public async Task<string> GetAccessToken()
        {
            var canvLogin = await ecatContext.CanvasLogins.Where(cl => cl.PersonId == loggedInUserId).SingleOrDefaultAsync();

            if (canvLogin == null)
            {
                return null;
            }

            if (canvLogin.AccessToken == null)
            {
                //there isn't a time where we will have a refresh token but no access token unless something weird happened, so just have the user re-auth with canvas
                return null;
            }

            //get a new token if we are within 3 minutes of expiriation or don't know expiration(?)
            //3 is close, but any call we are doing shouldn't take that long
            if (canvLogin.TokenExpires == null || canvLogin.TokenExpires < DateTime.Now.AddMinutes(3))
            {
                if (canvLogin.RefreshToken == null)
                {
                    //have an expired access token, but no refresh token
                    return null;
                }

                var client = new HttpClient();
                var authAddr = new Uri(canvasTokenUrl);
                var content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("refresh_token", canvLogin.RefreshToken)
                });

                var response = await client.PostAsync(authAddr, content);
                if (!response.IsSuccessStatusCode)
                {
                    //for some reason our refresh token isn't working
                    return null;
                }

                var respContent = await response.Content.ReadAsStringAsync();
                var respObject = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(respContent);

                canvLogin.AccessToken = respObject.access_token;
                canvLogin.TokenExpires = DateTime.Now.AddSeconds(respObject.expires_in);
                ecatContext.Entry(canvLogin).State = System.Data.Entity.EntityState.Modified;

                await ecatContext.SaveChangesAsync();
            }

            return canvLogin.AccessToken;
        }
    }
}
