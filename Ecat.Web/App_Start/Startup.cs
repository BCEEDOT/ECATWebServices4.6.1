using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using System.Web.Routing;
using Ecat.Web;
using Ecat.Web.Provider;
using Elmah.Contrib.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi;
using Ninject.Web.WebApi.OwinHost;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Ecat.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            #region Add/Remove Filters
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Filters.Add(new NoCacheHeaderFilter());
            #endregion

            #region Service-wide logging
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
            #endregion

            #region MVC Route Configuration
            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            RouteTable.Routes.MapRoute(name: "Test",
             url: "{Lti}/{Ping}/{site}",
             defaults: new { controller = "Lti", action = "Ping", id = UrlParameter.Optional });
            RouteTable.Routes.MapRoute(name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Lti", action = "Ping", id = UrlParameter.Optional });
            #endregion

            #region Api Route Configuration
            config.Routes.MapHttpRoute(
                name: "BreezeApi",
                routeTemplate: "breeze/{controller}/{action}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            //config.Routes.MapHttpRoute(
            //    name: "NotFound",
            //    routeTemplate: "{*path}",
            //    defaults: new {controller = "Error", action = "NotFound"}
            //    );
            #endregion

            #region Add/Remove Formatters
            var xmlSuptTypes = config.Formatters.XmlFormatter.SupportedMediaTypes;
            var xmlTypes  = xmlSuptTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            if (xmlTypes != null) 
            {
                xmlSuptTypes.Remove(xmlTypes);
            }
            #endregion

            ConfigureOauth(app);
            app.UseNinjectMiddleware(NinjectConfig.GetKernal);
            app.UseNinjectWebApi(config);
        }

        public void ConfigureOauth(IAppBuilder app)
        {
            AuthServerOptions.OabOpts = new OAuthBearerAuthenticationOptions();
            var serverOpts = new AuthServerOptions();
            app.UseOAuthAuthorizationServer(serverOpts.OauthOpts);
            app.UseOAuthBearerAuthentication(AuthServerOptions.OabOpts);
        }
    }
}
