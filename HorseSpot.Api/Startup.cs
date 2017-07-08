using HorseSpot.Api.App_Start;
using HorseSpot.Api.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

[assembly: OwinStartup(typeof(HorseSpot.Api.Startup))]
namespace HorseSpot.Api
{
    public class Startup
    {
        /// <summary>
        /// Application Configuration
        /// </summary>
        /// <param name="app">Application Builder</param>
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

        /// <summary>
        /// Configuration for OAuth
        /// </summary>
        /// <param name="app">Aplication Builder</param>
        public void ConfigureOAuth(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(6),
                Provider = new HorseSpotAuthorizationServerProvider(),
                RefreshTokenProvider = new HorseSpotRefreshTokenProvider(),
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}