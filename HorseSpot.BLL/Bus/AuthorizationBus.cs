using System.Collections.Generic;
using System.Threading.Tasks;
using HorseSpot.BLL.Converters;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Models.Models;
using Microsoft.AspNet.Identity;
using HorseSpot.Infrastructure.MailService;
using System.Configuration;

namespace HorseSpot.BLL.Bus
{
    public class AuthorizationBus : IAuthorizationBus
    {
        #region Local Variables

        private IUserDao _iUserDao;
        private IMailerService _iMailerService;
        private IRefreshTokenDao _iRefreshTokenDao;
        private IClientDao _iClientDao;

        #endregion

        #region Constructor

        public AuthorizationBus(IUserDao iUserDao, IMailerService iMailerService, IRefreshTokenDao iRefreshTokenDao, IClientDao iClientDao)
        {
            _iUserDao = iUserDao;
            _iMailerService = iMailerService;
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

        public async Task<UserModel> CreateExternalUser(RegisterExternalBindingModel model)
        {
            var user = new UserModel
            {
                UserName = model.UserName,
                FirstName = model.FirstName, 
                LastName = model.LastName,
                Email = model.Email,
                ImagePath = model.ImageUrl
            };

            await _iUserDao.CreateAsync(user);

            return user;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            return await _iUserDao.AddLoginAsync(userId, login);
        }

        public async Task SendWelcomeEmail(UserModel user)
        {
            EmailModel emailModel = new EmailModel
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = user.Email,
                ReceiverFirstName = user.FirstName,
                ReceiverLastName = user.LastName,
                EmailSubject = EmailSubjects.WelecomeSubject,
                EmailTemplatePath = EmailTemplatesPath.WelcomeTemplate
            };

            await _iMailerService.SendMail(emailModel);
        }

        #endregion
    }
}
