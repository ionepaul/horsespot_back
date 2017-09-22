using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Security;
using HorseSpot.BLL.Converters;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.DAL.Models;
using HorseSpot.Infrastructure.Constants;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.MailService;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Infrastructure.Validators;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Bus
{
    public class UserBus : IUserBus
    {
        #region Local Variables

        private IMailerService _iMailerService;
        private IUserDao _iUserDao;

        #endregion

        #region Constructor

        public UserBus(IUserDao iAuthDao, IMailerService iMailerService)
        {
            _iMailerService = iMailerService;
            _iUserDao = iAuthDao;
        }

        #endregion

        #region Public Methods

        public async Task<UserViewModel> RegisterUser(UserViewModel user)
        {
            ValidateUserRegistration(user);

            var userModel = UserConverter.ConvertUserViewModelToUserModel(user);

            var userDboAfterSave = await _iUserDao.RegisterUser(userModel, user.Password);
            
            EmailModel emailModel = new EmailModel
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = user.Email,
                ReceiverFirstName = user.FirstName,
                ReceiverLastName = user.LastName,
                EmailSubject = EmailSubjects.WelecomeSubject,
                EmailTemplatePath = EmailTemplatesPath.WelcomeTemplate
            };

            if (user.NewsletterSubscription.HasValue && user.NewsletterSubscription.Value)
            {
                var subscriber = new Subscriber(user.Email);
                _iUserDao.RegisterToNewsletter(subscriber);
            }

            await _iMailerService.SendMail(emailModel);

            return UserConverter.FromUserModelToUserViewModel(userDboAfterSave);
        }

        public async Task<UserDTO> EditProfile(string id, EditProfileViewModel editProfile)
        {
            UserModel userModel = _iUserDao.FindUserById(id);

            if (userModel == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidUserIdentifier);
            }

            return await UpdateUserProfile(userModel, editProfile);
        }

        public async Task ChangePassword(string userId, ChangePasswordViewModel changePassword)
        {
            UserModel userModel = _iUserDao.FindUserById(userId);

            if (userModel == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidUserIdentifier);
            }
   
            await ValidateChangePassword(userModel, changePassword);

            await _iUserDao.ChangeUserPassword(userId, changePassword.NewPassword);
        }

        public IEnumerable<UserViewModel> GetAllUsers()
        {
            var users = _iUserDao.GetAllUsers();

            return users.Select(UserConverter.FromUserModelToUserViewModel);
        }

        public UserDTO GetUserDetails(string id)
        {
            var user = _iUserDao.FindUserById(id);

            if (user == null)
            {
                throw new ValidationException(Resources.InvalidUserIdentifier);
            }

            return UserConverter.FromUserModelToUserDTO(user);
        }

        public async Task<bool> CheckIfAdmin(string userId)
        {
            var userRoles = await _iUserDao.UserRoles(userId);

            return userRoles.Contains(ApplicationConstants.ADMIN);
        }

        public async Task Delete(string userId)
        {
            UserModel userModel = _iUserDao.FindUserById(userId);

            if (userModel == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidUserIdentifier);
            }

            await _iUserDao.DeleteUser(userModel);
        }

        public async Task ForgotPassword(string email)
        {
            if (email == null)
            {
                throw new ValidationException(Resources.EmailDoesNotExist);
            }

            var user = _iUserDao.FindUserByEmail(email);

            if (user == null)
            {
                throw new ValidationException(Resources.EmailDoesNotExist);
            }

            var temporaryPassowrd = Membership.GeneratePassword(10, 0);

            await _iUserDao.ChangeUserPassword(user.Id, temporaryPassowrd);

            EmailModel emailModel = new EmailModel
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = user.Email,
                ReceiverFirstName = user.FirstName,
                ReceiverLastName = user.LastName,
                TemporaryPassword = temporaryPassowrd,
                EmailSubject = EmailSubjects.ForgotPassword,
                EmailTemplatePath = EmailTemplatesPath.ForgotPassword
            };

            await _iMailerService.SendMail(emailModel);
        }

        public void SubscribeToNewsletter(string email)
        {
            Regex emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

            if (!emailRegex.IsMatch(email))
            {
                throw new ValidationException(Resources.InvalidEmailFormat);
            }

            var existingSubscriber = _iUserDao.FindSubscriberByEmail(email);

            if (existingSubscriber != null)
            {
                throw new ConflictException(Resources.AlreadyRegistredSubscriber);
            }

            var subscriber = new Subscriber(email);

            _iUserDao.RegisterToNewsletter(subscriber);
        }

        public GetHorseAdListResultsDTO GetAllForUser(int pageNumber, string userId)
        {
            var user = _iUserDao.FindUserById(userId);

            if (user == null)
            {
                throw new ValidationException(Resources.InvalidUserIdentifier);
            }

            var skipNumber = GetNumberToSkip(pageNumber);
            var usersHorseAds = user.HorseAds.Where(x => !x.IsDeleted && !x.IsSold);

            var results = new GetHorseAdListResultsDTO();
            results.TotalCount = usersHorseAds.Count();
            results.HorseAdList = usersHorseAds.Skip(skipNumber).Take(ApplicationConstants.AdsPerPage).Select(HorseAdConverter.FromHorseAdToHorseAdListModel);

            return results;
        }

        public GetHorseAdListResultsDTO GetReferencesForUser(int pageNumber, string userId)
        {
            var user = _iUserDao.FindUserById(userId);

            if (user == null)
            {
                throw new ValidationException(Resources.InvalidUserIdentifier);
            }

            var skipNumber = GetNumberToSkip(pageNumber);
            var usersHorseAds = user.HorseAds.Where(x => x.IsSold);

            var results = new GetHorseAdListResultsDTO();
            results.TotalCount = usersHorseAds.Count();
            results.HorseAdList = usersHorseAds.Skip(skipNumber).Take(ApplicationConstants.AdsPerPage).Select(HorseAdConverter.FromHorseAdToHorseAdListModel);

            return results;
        }

        public GetHorseAdListResultsDTO GetUserFavorites(int pageNumber, string userId)
        {
            var user = _iUserDao.FindUserById(userId);

            if (user == null)
            {
                throw new ValidationException(Resources.InvalidUserIdentifier);
            }

            var skipNumber = GetNumberToSkip(pageNumber);
            var usersHorseAds = user.FavoriteHorseAds.Where(x => !x.IsDeleted).Select(x => x.FavoriteHorseAd);

            var results = new GetHorseAdListResultsDTO();
            results.TotalCount = usersHorseAds.Count();
            results.HorseAdList = usersHorseAds.Skip(skipNumber).Take(ApplicationConstants.AdsPerPage).Select(HorseAdConverter.FromHorseAdToHorseAdListModel);

            return results;
        }

        #endregion

        #region Private Methods

        private async Task ValidateChangePassword(UserModel user, ChangePasswordViewModel changePassword)
        {
            ValidationHelper.ValidateModelAttributes<ChangePasswordViewModel>(changePassword);

            var getUserByCurrentPassowrd = await _iUserDao.FindUser(user.UserName, changePassword.CurrentPassword);

            if (getUserByCurrentPassowrd == null)
            {
                throw new ConflictException(Resources.InccorectCurrentPassword);
            }

            if (changePassword.NewPassword != changePassword.ConfirmPassword)
            {
                throw new ValidationException(Resources.ConfirmPasswordNotMatch);
            }
        }

        private void ValidateUserRegistration(UserViewModel user)
        {
            if (user == null)
            {
                throw new ValidationException(Resources.InvalidRegistrationRequest);
            }

            ValidationHelper.ValidateModelAttributes<UserViewModel>(user);

            Regex emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
            Regex phoneNumberRegex = new Regex(@"^(\+\d{1,3}[- ]?)?\d{10}$");

            if (!emailRegex.IsMatch(user.Email))
            {
                throw new ValidationException(Resources.InvalidEmailFormat);
            }

            if (user.Password.Length < 6)
            {
                throw new ValidationException(Resources.InvalidPasswordFormat);
            }

            if (user.Password != user.ConfirmPassword)
            {
                throw new ValidationException(Resources.InvalidConfirmPasswordFormat);
            }

            if (!phoneNumberRegex.IsMatch(user.PhoneNumber))
            {
                throw new ValidationException(Resources.InvalidPhoneNumberFormat);
            }

            if (!FindUserByEmail(user.Email))
            {
                throw new ConflictException(Resources.ExistUserEmail);
            }
        }

        private async Task<UserDTO> UpdateUserProfile(UserModel user, EditProfileViewModel editProfile)
        {
            user.FirstName = (editProfile.FirstName != null) ? editProfile.FirstName : user.FirstName;
            user.LastName = (editProfile.LastName != null) ? editProfile.LastName : user.LastName;

            Regex phoneNumberRegex = new Regex(@"^(\+\d{1,3}[- ]?)?\d{10}$");

            if (editProfile.PhoneNumber != null && !phoneNumberRegex.IsMatch(user.PhoneNumber))
            {
                throw new ValidationException(Resources.InvalidPhoneNumberFormat);
            }

            user.PhoneNumber = (editProfile.PhoneNumber != null) ? editProfile.PhoneNumber : user.PhoneNumber;

            var editedUser = await _iUserDao.UpdateUser(user);

            return UserConverter.FromUserModelToUserDTO(user);
        }

        private bool FindUserByEmail(string email)
        {
            UserModel user = _iUserDao.FindUserByEmail(email);

            if (user == null)
            {
                return true;
            }

            return false;
        }

        private int GetNumberToSkip(int pageNumber)
        {
            return (pageNumber - 1) * ApplicationConstants.AdsPerPage;
        }

        #endregion
    }
}
