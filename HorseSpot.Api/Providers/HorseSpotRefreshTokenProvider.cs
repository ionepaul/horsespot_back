using System;
using System.Threading.Tasks;
using System.Web.Http;
using HorseSpot.BLL.Interfaces;
using HorseSpot.Infrastructure.Utils;
using HorseSpot.Models.Models;
using Microsoft.Owin.Security.Infrastructure;

namespace HorseSpot.Api.Providers
{
    public class HorseSpotRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private IAuthorizationBus _iAuthorizationBus;

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

            //TicketSerializer serializer = new TicketSerializer();
            //token.ProtectedTicket = System.Text.Encoding.Default.GetString(serializer.Serialize(context.Ticket));

            token.ProtectedTicket = context.SerializeTicket();

            var result = await _iAuthorizationBus.AddRefreshToken(token);

            if (result)
            {
                context.SetToken(refreshTokenId);
            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            _iAuthorizationBus = (IAuthorizationBus)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAuthorizationBus));

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = Helper.GetHash(context.Token);

            var refreshToken = await _iAuthorizationBus.FindRefreshToken(hashedTokenId);

            if (refreshToken != null)
            {
                //TicketSerializer serializer = new TicketSerializer();
                //context.SetTicket(serializer.Deserialize(System.Text.Encoding.Default.GetBytes(refreshToken.ProtectedTicket)));
                context.DeserializeTicket(refreshToken.ProtectedTicket);

                var result = await _iAuthorizationBus.RemoveRefreshToken(hashedTokenId);
            }
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }
    }
}