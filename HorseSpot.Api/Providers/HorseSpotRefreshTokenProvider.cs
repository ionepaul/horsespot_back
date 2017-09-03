using HorseSpot.BLL.Interfaces;
using HorseSpot.Infrastructure.Utils;
using HorseSpot.Models.Models;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace HorseSpot.Api.Providers
{
    public class HorseSpotRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private IAuthorizationBus _iAuthorizationBus;

        /// <summary>
        /// Creates Refresh Token
        /// </summary>
        /// <param name="context">Authentication Token Context</param>
        /// <returns>Task</returns>
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            _iAuthorizationBus = (IAuthorizationBus)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAuthorizationBus));

            var clientId = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientId))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");

            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            var token = new RefreshTokenDTO()
            {
                Id = Helper.GetHash(refreshTokenId),
                ClientId = clientId,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            var result = await _iAuthorizationBus.AddRefreshToken(token);

            if (result)
            {
                context.SetToken(refreshTokenId);
            }
        }

        /// <summary>
        /// Receive Refresh Token
        /// </summary>
        /// <param name="context">Authentication Token Receive Context</param>
        /// <returns>Task</returns>
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            _iAuthorizationBus = (IAuthorizationBus)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAuthorizationBus));

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = Helper.GetHash(context.Token);

            var refreshToken = await _iAuthorizationBus.FindRefreshToken(hashedTokenId);

            if (refreshToken != null)
            {
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                var result = await _iAuthorizationBus.RemoveRefreshToken(hashedTokenId);
            }
        }

        /// <summary>
        /// Receive
        /// </summary>
        /// <param name="context">Context</param>
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="context">Context</param>
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }
    }
}