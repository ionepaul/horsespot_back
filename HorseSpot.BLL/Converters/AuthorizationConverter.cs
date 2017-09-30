using HorseSpot.DAL.Entities;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Converters
{
    public static class AuthorizationConverter
    {
        public static ClientDTO FromClientToClientDTO(Client client)
        {
            if (client == null)
            {
                return null;
            }

            return new ClientDTO
            {
                Id = client.Id,
                Active = client.Active,
                AllowedOrigin = client.AllowedOrigin,
                ApplicationType = client.ApplicationType,
                Name = client.Name,
                RefreshTokenLifeTime = client.RefreshTokenLifeTime,
                Secret = client.Secret
            };
        }

        public static RefreshTokenDTO FromRefreshTokenToRefreshTokenDTO(RefreshToken token)
        {
            if (token == null)
            {
                return null;
            }

            return new RefreshTokenDTO
            {
                Id = token.Id,
                ClientId = token.ClientId,
                ExpiresUtc = token.ExpiresUtc,
                IssuedUtc = token.IssuedUtc,
                ProtectedTicket = token.ProtectedTicket,
                Subject = token.Subject
            };
        }

        public static RefreshToken FromRefreshTokenDTOToRefreshToken(RefreshTokenDTO token)
        {
            if (token == null)
            {
                return null;
            }

            return new RefreshToken
            {
                Id = token.Id,
                ClientId = token.ClientId,
                ExpiresUtc = token.ExpiresUtc,
                IssuedUtc = token.IssuedUtc,
                ProtectedTicket = token.ProtectedTicket,
                Subject = token.Subject
            };
        }
    }
}
