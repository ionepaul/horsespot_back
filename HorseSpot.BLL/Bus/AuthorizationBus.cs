using HorseSpot.BLL.Converters;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HorseSpot.BLL.Bus
{
    public class AuthorizationBus : IAuthorizationBus
    {
        #region Local Variables

        private IUserDao _iUserDao;
        private IRefreshTokenDao _iRefreshTokenDao;
        private IClientDao _iClientDao;

        #endregion

        #region Constructor

        /// <summary>
        /// AuthorizationBus Contstructor
        /// </summary>
        /// <param name="iUserDao">UserDao Interface</param>
        /// <param name="iRefreshTokenDao">RefreshTokenDao Interface</param>
        /// <param name="iClientDao">ClientDao Interface</param>
        public AuthorizationBus(IUserDao iUserDao, IRefreshTokenDao iRefreshTokenDao, IClientDao iClientDao)
        {
            _iUserDao = iUserDao;
            _iRefreshTokenDao = iRefreshTokenDao;
            _iClientDao = iClientDao;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds user by email and password
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <returns>User View Model</returns>
        public async Task<UserViewModel> FindUser(string email, string password)
        {
            var userModel = await _iUserDao.FindUser(email, password);

            return UserConverter.FromUserModelToUserViewModel(userModel);
        }

        /// <summary>
        /// Find an application type client by id
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <returns>ClientDTO Model</returns>
        public ClientDTO FindClient(string clientId)
        {
            var client = _iClientDao.FindClient(clientId);

            return AuthorizationConverter.FromClientToClientDTO(client);
        }

        /// <summary>
        /// Find the Refresh Token by hashed token Id
        /// </summary>
        /// <param name="hashedTokenId">Hashed Token Id</param>
        /// <returns>RefreshTokenDTO Model</returns>
        public async Task<RefreshTokenDTO> FindRefreshToken(string hashedTokenId)
        {
            var token = await _iRefreshTokenDao.FindRefreshToken(hashedTokenId);

            return AuthorizationConverter.FromRefreshTokenToRefreshTokenDTO(token);
        }

        /// <summary>
        /// Add a refresh token
        /// </summary>
        /// <param name="token">Refresh Token Model</param>
        /// <returns>True/False</returns>
        public async Task<bool> AddRefreshToken(RefreshTokenDTO token)
        {
            RefreshToken refrehToken = AuthorizationConverter.FromRefreshTokenDTOToRefreshToken(token);

            if (refrehToken == null)
            {
                throw new ValidationException();
            }

            return await _iRefreshTokenDao.AddRefreshToken(refrehToken);
        }

        /// <summary>
        /// Gets the roles for a user
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>List of user roles</returns>
        public async Task<IList<string>> UserRoles(string id)
        {
            return await _iUserDao.UserRoles(id);
        }

        /// <summary>
        /// Remove Refresh Token by hash token id
        /// </summary>
        /// <param name="hashedTokenId">Hash Token Id</param>
        /// <returns>True/False</returns>
        public async Task<bool> RemoveRefreshToken(string hashedTokenId)
        {
            return await _iRefreshTokenDao.RemoveRefreshToken(hashedTokenId);
        }

        #endregion
    }
}
