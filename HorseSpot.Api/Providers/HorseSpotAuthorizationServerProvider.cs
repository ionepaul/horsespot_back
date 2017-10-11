using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using HorseSpot.BLL.Interfaces;
using HorseSpot.Infrastructure.Constants;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Infrastructure.Utils;
using HorseSpot.Models.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace HorseSpot.Api.Providers
{
    public class HorseSpotAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private IAuthorizationBus _iAuthorizationBus;

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            _iAuthorizationBus = (IAuthorizationBus)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAuthorizationBus));

            string clientId = string.Empty;
            string clientSecret = string.Empty;
            ClientDTO client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                context.Validated();
                return Task.FromResult<object>(null);
            }

            client = _iAuthorizationBus.FindClient(context.ClientId);


            if (client == null)
            {
                context.SetError("invalid_clientId");
                return Task.FromResult<object>(null);
            }

            if (client.ApplicationType == ApplicationTypes.Client_NativeConfidentialApplication)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                else
                {
                    if (client.Secret != Helper.GetHash(clientSecret))
                    {
                        context.SetError("invalid_clientId", "client secret is invalid");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive");
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            _iAuthorizationBus = (IAuthorizationBus)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAuthorizationBus));

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            if (allowedOrigin == null)
            {
                allowedOrigin = "*";
            }

            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            if (context.UserName == null || context.Password == null)
            {
                context.SetError("invalid_request", Resources.InvalidLoginRequest);
                return;
            } 

            var user = await _iAuthorizationBus.FindUser(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", Resources.InvalidEmailOrPassword);
                return;
            }

            var userRoles = await _iAuthorizationBus.UserRoles(user.Id);

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim("UserId", user.Id));

            foreach (var role in userRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                {
                    "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                },
                {
                    "username", context.UserName
                },
                {
                    "isAdmin", (userRoles.Contains("Admin")) ? "true" : "false"
                },
                {
                    "userId", user.Id
                },
                {
                    "firstName", user.FirstName
                },
                {
                    "profilePic", user.ProfileImage
                }
            });

            var ticket = new AuthenticationTicket(identity, props);

            context.Validated(ticket);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            _iAuthorizationBus = (IAuthorizationBus)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAuthorizationBus));

            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClinet = context.ClientId;

            if (originalClient != currentClinet)
            {
                context.SetError("invalid_clientId", "refresh token is issued to a different clientId");
                return Task.FromResult<object>(null);
            }

            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}