using HorseSpot.DAL.Entities;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Converters
{
    /// <summary>
    /// Static class used to map the authorization entites database model to domain trasfer objects and vice-versa
    /// </summary>
    public static class AuthorizationConverter
    {
        /// <summary>
        /// Converts client database model to client domain object model
        /// </summary>
        /// <param name="client">Client Database Model</param>
        /// <returns>ClientDTO Model</returns>
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

        /// <summary>
        /// Converts Refresh Token database object to RefreshTokenDTO
        /// </summary>
        /// <param name="token">RefreshToke database model</param>
        /// <returns>RefreshTokenDTO Model</returns>
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

        /// <summary>
        /// Converts RefreshTokenDTO Model to RefreshToken database Model
        /// </summary>
        /// <param name="token">RefreshTokenDTO Model</param>
        /// <returns>RefreshToken Database Model</returns>
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
