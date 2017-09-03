using HorseSpot.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HorseSpot.DAL.Interfaces
{
    public interface IRefreshTokenDao : IDao<RefreshToken>
    {
        Task<bool> AddRefreshToken(RefreshToken token);
        Task<bool> RemoveRefreshToken(string refreshTokenId);
        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> FindRefreshToken(string refreshTokenId);
        List<RefreshToken> GetAllRefreshTokens();
    }
}
