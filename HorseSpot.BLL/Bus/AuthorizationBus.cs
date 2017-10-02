using System.Collections.Generic;
using System.Threading.Tasks;
using HorseSpot.BLL.Converters;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Models.Models;
using Microsoft.AspNet.Identity;

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

        public AuthorizationBus(IUserDao iUserDao, IRefreshTokenDao iRefreshTokenDao, IClientDao iClientDao)
        {
            _iUserDao = iUserDao;
            _iRefreshTokenDao = iRefreshTokenDao;
            _iClientDao = iClientDao;
        }

        #endregion

        #region Public Methods

        public async Task<UserViewModel> FindUser(string email, string password)
        {
            var userModel = await _iUserDao.FindUser(email, password);

            return UserConverter.FromUserModelToUserViewModel(userModel);
        }

        public ClientDTO FindClient(string clientId)
        {
            var client = _iClientDao.FindClient(clientId);

            return AuthorizationConverter.FromClientToClientDTO(client);
        }

        public async Task<RefreshTokenDTO> FindRefreshToken(string hashedTokenId)
        {
            var token = await _iRefreshTokenDao.FindRefreshToken(hashedTokenId);

            return AuthorizationConverter.FromRefreshTokenToRefreshTokenDTO(token);
        }

        public async Task<bool> AddRefreshToken(RefreshTokenDTO token)
        {
            RefreshToken refrehToken = AuthorizationConverter.FromRefreshTokenDTOToRefreshToken(token);

            if (refrehToken == null)
            {
                throw new ValidationException();
            }

            return await _iRefreshTokenDao.AddRefreshToken(refrehToken);
        }

        public async Task<IList<string>> UserRoles(string id)
        {
            return await _iUserDao.UserRoles(id);
        }

        public async Task<bool> RemoveRefreshToken(string hashedTokenId)
        {
            return await _iRefreshTokenDao.RemoveRefreshToken(hashedTokenId);
        }

        public Task<UserModel> FindUserByLoginInfo(UserLoginInfo loginInfo)
        {
            return _iUserDao.FindUserByLoginInfo(loginInfo);
        }

        public async Task<IdentityResult> CreateExternalUser(string userName)
        {
            var user = new UserModel { UserName = userName };

            return await _iUserDao.CreateAsync(user);
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            return await _iUserDao.AddLoginAsync(userId, login);
        }

        #endregion
    }
}
