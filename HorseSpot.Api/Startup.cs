using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using HorseSpot.Api.App_Start;
using HorseSpot.Api.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(HorseSpot.Api.Startup))]
namespace HorseSpot.Api
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static FacebookAuthenticationOptions FacebookAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            var unityContainer = Bootstrapper.Initialise();

            ConfigureOAuth(app);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            app.UseWebApi(config);

            JobScheduler.Start();
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(6),
                Provider = new HorseSpotAuthorizationServerProvider(),
                RefreshTokenProvider = new HorseSpotRefreshTokenProvider(),
            };

            //Configure OAuth Berar Token
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            //Configur Facebook external login
            FacebookAuthOptions = new FacebookAuthenticationOptions()
            {
                AppId = "xx",
                AppSecret = "xx",
                Provider = new FacebookAuthProvider()
            };

            app.UseFacebookAuthentication(FacebookAuthOptions);
        }

    }
}