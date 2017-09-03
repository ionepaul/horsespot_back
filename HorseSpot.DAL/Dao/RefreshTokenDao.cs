using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        /// <summary>
        /// Add refresh token to the database, remove if exists for the same subject and client and then add it
        /// </summary>
        /// <param name="token">Refresh Token Model</param>
        /// <returns>True/False</returns>
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

        /// <summary>
        /// Remove Refresh Token from the database by id
        /// </summary>
        /// <param name="refreshTokenId">Refresh Token Id</param>
        /// <returns>True/False</returns>
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

        /// <summary>
        /// Remove Refresh Token From the database by entity
        /// </summary>
        /// <param name="refreshToken">Refresh Token Entity</param>
        /// <returns>True/False</returns>
        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _dbset.Remove(refreshToken);

            return await _ctx.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Find refresh token in the database by id
        /// </summary>
        /// <param name="refreshTokenId">Refresh Token Id</param>
        /// <returns>Refresh Token Model</returns>
        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _dbset.FindAsync(refreshTokenId);

            return refreshToken;
        }

        /// <summary>
        /// Gets all refresh tokens from the database
        /// </summary>
        /// <returns>List of refresh tokens</returns>
        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _dbset.ToList();
        }

        #endregion
    }
}
