﻿using System;
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

        private readonly IMailerService _iMailerService;
        private readonly IUserDao _iUserDao;
        private readonly IHorseAdDao _iHorseAdDao;

        #endregion

        #region Constructor

        public UserBus(IUserDao iUserDao, IMailerService iMailerService, IHorseAdDao iHorseAdDao)
        {
            _iMailerService = iMailerService;
            _iUserDao = iUserDao;
            _iHorseAdDao = iHorseAdDao;
        }

        #endregion

        #region Public Methods

        public async Task<UserViewModel> RegisterUser(UserViewModel user)
        {
            ValidateUserRegistration(user);

            var userModel = UserConverter.ConvertUserViewModelToUserModel(user);

            var savedUser = await _iUserDao.RegisterUser(userModel, user.Password);

            if (user.NewsletterSubscription.HasValue && user.NewsletterSubscription.Value)
            {
                var subscriber = new Subscriber(user.Email);

                _iUserDao.RegisterToNewsletter(subscriber);
            }

            await SendWelcomeEmail(userModel);

            return UserConverter.FromUserModelToUserViewModel(savedUser);
        }

        public UserDTO FindUserByEmail(string email)
        {
            var userModel = _iUserDao.FindUserByEmail(email);

            //Do not check for user null used to see if external user has local account
            //CheckIfUserExists(userModel);

            return UserConverter.FromUserModelToUserDTO(userModel);
        }

        public async Task<UserDTO> EditProfile(string id, EditProfileViewModel editProfile)
        {
            UserModel userModel = _iUserDao.FindUserById(id);

            CheckIfUserExists(userModel);

            return await UpdateUserInformation(userModel, editProfile);
        }

        public async Task ChangePassword(string userId, ChangePasswordViewModel changePassword)
        {
            UserModel userModel = _iUserDao.FindUserById(userId);

            CheckIfUserExists(userModel);

            await ValidateChangePasswordModel(userModel, changePassword);

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

            CheckIfUserExists(user);

            return UserConverter.FromUserModelToUserDTO(user);
        }

        public async Task<bool> CheckIfAdmin(string userId)
        {
            var userRoles = await _iUserDao.UserRoles(userId);

            return userRoles.Contains(ApplicationConstants.ADMIN);
        }

        public async Task DeleteUserById(string userId)
        {
            var userModel = _iUserDao.FindUserById(userId);

            CheckIfUserExists(userModel);

            if (userModel.HorseAds?.Count > 0)
            {
                foreach (var horseAdsId in userModel.HorseAds.Select(x => x.Id))
                {
                    var horseAd = _iHorseAdDao.GetById(horseAdsId);

                    horseAd.IsDeleted = true;

                    await _iHorseAdDao.UpdateAsync(horseAd);
                }

                userModel.HorseAds.Clear();
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

            CheckIfUserExists(user);

            var temporaryPassword = Membership.GeneratePassword(10, 0);

            await _iUserDao.ChangeUserPassword(user.Id, temporaryPassword);

            await SendPasswordRecoveryEmail(user, temporaryPassword);
        }

        public void SubscribeToNewsletter(string email)
        {
            ValidateEmail(email);

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

            CheckIfUserExists(user);

            var userHorseAds = user.HorseAds.Where(x => !x.IsDeleted && !x.IsSold);

            return CreateHorseListResult(userHorseAds, pageNumber);
        }

        public GetHorseAdListResultsDTO GetReferencesForUser(int pageNumber, string userId)
        {
            var user = _iUserDao.FindUserById(userId);

            CheckIfUserExists(user);

            var userSoldHorseAds = user.HorseAds.Where(x => x.IsSold);

            return CreateHorseListResult(userSoldHorseAds, pageNumber);
        }

        public GetHorseAdListResultsDTO GetUserFavorites(int pageNumber, string userId)
        {
            var user = _iUserDao.FindUserById(userId);

            CheckIfUserExists(user);

            var userFavoriteHorseAds = user.FavoriteHorseAds.Where(x => !x.IsDeleted).Select(x => x.FavoriteHorseAd);

            return CreateHorseListResult(userFavoriteHorseAds, pageNumber);
        }

        public async Task SetUserProfilePicture(string path, string id)
        {
            var user = _iUserDao.FindUserById(id);

            CheckIfUserExists(user);

            user.ImagePath = path;

            await _iUserDao.UpdateUser(user);
        }

        public UserFullProfile GetUserFullProfile(string userId)
        {
            var user = _iUserDao.FindUserById(userId);

            CheckIfUserExists(user);

            var userFullProfile = UserConverter.FromUserModelToUserFullProfile(user);

            return userFullProfile;
        }


        #endregion

        #region Private Methods

        private async Task ValidateChangePasswordModel(UserModel user, ChangePasswordViewModel changePassword)
        {
            ValidationHelper.ValidateModelAttributes<ChangePasswordViewModel>(changePassword);

            var userByCredentials = await _iUserDao.FindUser(user.UserName, changePassword.CurrentPassword);

            if (userByCredentials == null)
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

            if (user.TermsAccepted != true)
            {
                throw new ValidationException(Resources.InvalidRegistrationRequest);
            }

            if (user.PrivacyPolicyAccepted != true)
            {
                throw new ValidationException(Resources.InvalidRegistrationRequest);
            }

            ValidationHelper.ValidateModelAttributes<UserViewModel>(user);

            Regex phoneNumberRegex = new Regex(@"^(?=.*[0-9])[- +()0-9].*[0-9]+$");

            ValidateEmail(user.Email);

            if (string.IsNullOrEmpty(user.FirstName))
            {
                throw new ValidationException(Resources.InvalidRegistrationRequest);
            }

            if (user.Password.Length < 6)
            {
                throw new ValidationException(Resources.InvalidPasswordFormat);
            }

            if (user.Password != user.ConfirmPassword)
            {
                throw new ValidationException(Resources.InvalidConfirmPasswordFormat);
            }

            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                if (!phoneNumberRegex.IsMatch(user.PhoneNumber) || user.PhoneNumber.Length < 5)
                {
                    throw new ValidationException(Resources.InvalidPhoneNumberFormat);
                }
            }

            if (!CheckIfUserExistsByEmail(user.Email))
            {
                throw new ConflictException(Resources.ExistUserEmail);
            }
        }

        private async Task<UserDTO> UpdateUserInformation(UserModel user, EditProfileViewModel editProfile)
        {
            user.FirstName = editProfile.FirstName ?? user.FirstName;
            user.LastName = editProfile.LastName ?? user.LastName;

            Regex phoneNumberRegex = new Regex(@"^(?=.*[0-9])[- +()0-9].*[0-9]+$");

            if (!string.IsNullOrEmpty(editProfile.PhoneNumber))
            {
                if (!phoneNumberRegex.IsMatch(editProfile.PhoneNumber) || editProfile.PhoneNumber.Length < 5)
                {
                    throw new ValidationException(Resources.InvalidPhoneNumberFormat);
                }
            }

            if (editProfile.TermsAccepted != true)
            {
                throw new ValidationException(Resources.InvalidExternalLoginRequest);
            }

            if (editProfile.PrivacyPolicyAccepted != true)
            {
                throw new ValidationException(Resources.InvalidExternalLoginRequest);
            }

            user.PrivacyPolicyAccepted = editProfile.PrivacyPolicyAccepted;
            user.TermsAccepted = editProfile.TermsAccepted;
            user.PhoneNumber = editProfile.PhoneNumber;
            user.NewsletterSubscription = editProfile.NewsletterSubscription;
            user.DisplayEmail = editProfile.DisplayEmail;
            user.DisplayPhoneNumber = editProfile.DisplayPhoneNumber;

            var editedUser = await _iUserDao.UpdateUser(user);

            return UserConverter.FromUserModelToUserDTO(user);
        }

        private bool CheckIfUserExistsByEmail(string email)
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

        private void CheckIfUserExists(UserModel userModel)
        {
            if (userModel == null)
            {
                throw new ValidationException(Resources.InvalidUserIdentifier);
            }
        }

        private void ValidateEmail(string email)
        {
            Regex emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

            if (!emailRegex.IsMatch(email))
            {
                throw new ValidationException(Resources.InvalidEmailFormat);
            }
        }

        private GetHorseAdListResultsDTO CreateHorseListResult(IEnumerable<HorseAd> horseAdList, int pageNumber)
        {
            var skipNumber = GetNumberToSkip(pageNumber);

            var horseAdListResult = new GetHorseAdListResultsDTO()
            {
                TotalCount = horseAdList.Count(),
                HorseAdList = horseAdList.Skip(skipNumber).Take(ApplicationConstants.AdsPerPage).Select(HorseAdConverter.FromHorseAdToHorseAdListModel)
            };

            return horseAdListResult;
        }

        private async Task SendWelcomeEmail(UserModel user)
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

        private async Task SendPasswordRecoveryEmail(UserModel user, string temporaryPassword)
        {
            EmailModel emailModel = new EmailModel
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = user.Email,
                ReceiverFirstName = user.FirstName,
                ReceiverLastName = user.LastName,
                TemporaryPassword = temporaryPassword,
                EmailSubject = EmailSubjects.ForgotPassword,
                EmailTemplatePath = EmailTemplatesPath.ForgotPassword
            };

            await _iMailerService.SendMail(emailModel);
        }

        #endregion
    }
}
