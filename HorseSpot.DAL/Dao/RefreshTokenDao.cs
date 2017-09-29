using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;

namespace HorseSpot.DAL.Dao
{
    public class RefreshTokenDao : AbstractDao<RefreshToken>, IRefreshTokenDao
    {
        #region Constructor

        public RefreshTokenDao(HorseSpotDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion

        #region Public Methods

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var existingToken = _dbset.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            _dbset.Add(token);

            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _dbset.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                _dbset.Remove(refreshToken);

                return await _ctx.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _dbset.Remove(refreshToken);

            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _dbset.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _dbset.ToList();
        }

        #endregion
    }
}
