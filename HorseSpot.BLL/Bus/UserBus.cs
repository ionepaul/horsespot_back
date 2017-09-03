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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Security;

namespace HorseSpot.BLL.Bus
{
    public class UserBus : IUserBus
    {
        #region Local Variables

        private IMailerService _iMailerService;
        private IUserDao _iUserDao;

        #endregion

        #region Constructor
        /// <summary>
        /// UserBus Constructor
        /// </summary>
        /// <param name="iAuthDao">Authorization Dao Interface</param>
        /// <param name="iMailerService">Mailer Service Interface</param>
        public UserBus(IUserDao iAuthDao, IMailerService iMailerService)
        {
            _iMailerService = iMailerService;
            _iUserDao = iAuthDao;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates user model, register user, sends mail in case of success
        /// </summary>
        /// <param name="user">User model</param>
        /// <returns>Exception if invalid data or registred user model</returns>
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

        /// <summary>
        /// Update an user profile
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="editProfile">User modified data</param>
        /// <returns>Exception if invalid data or updated user model</returns>
        public async Task<UserDTO> EditProfile(string id, EditProfileViewModel editProfile)
        {
            UserModel userModel = _iUserDao.FindUserById(id);

            if (userModel == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidUserIdentifier);
            }

            return await UpdateUserProfile(userModel, editProfile);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="changePassword">Change password required information</param>
        /// <returns>Exception if invalid data or task if success</returns>
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

        /// <summary>
        /// Retrieve all users
        /// </summary>
        /// <returns>IEnumerable of users</returns>
        public IEnumerable<UserViewModel> GetAllUsers()
        {
            var users = _iUserDao.GetAllUsers();

            return users.Select(UserConverter.FromUserModelToUserViewModel);
        }

        /// <summary>
        /// Retrieve user details
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Exception if not found, user details if success</returns>
        public UserDTO GetUserDetails(string id)
        {
            var user = _iUserDao.FindUserById(id);

            if (user == null)
            {
                throw new ValidationException(Resources.InvalidUserIdentifier);
            }

            return UserConverter.FromUserModelToUserDTO(user);
        }

        /// <summary>
        /// Check if user is admin
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>True/False</returns>
        public async Task<bool> CheckIfAdmin(string userId)
        {
            var userRoles = await _iUserDao.UserRoles(userId);

            return userRoles.Contains(ApplicationConstants.ADMIN);
        }

        /// <summary>
        /// Delete an user account
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Exception if not found, task if success</returns>
        public async Task Delete(string userId)
        {
            UserModel userModel = _iUserDao.FindUserById(userId);

            if (userModel == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidUserIdentifier);
            }

            await _iUserDao.DeleteUser(userModel);
        }

        /// <summary>
        /// Generates new password for a specific registred email and sends email with the password
        /// </summary>
        /// <param name="email">Registred email</param>
        /// <returns>Exception if invalid email, task if success</returns>
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

        /// <summary>
        /// Add an email to newsletter subscription list
        /// </summary>
        /// <param name="email">Email to add to list</param>
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates the data of the change password model
        /// </summary>
        /// <param name="user">User Model</param>
        /// <param name="changePassword">Change passowrd model</param>
        /// <returns>Exception or Task</returns>
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

        /// <summary>
        /// Validates the data of the user model when registrating
        /// </summary>
        /// <param name="user">User Model</param>
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

        /// <summary>
        /// Validates and Updates user model
        /// </summary>
        /// <param name="user">User Model</param>
        /// <param name="editProfile">Updated data</param>
        /// <returns>Exception or updated user model</returns>
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

        /// <summary>
        /// Check if the user exists in database by email
        /// </summary>
        /// <param name="email">Email to look for</param>
        /// <returns>True/False</returns>
        private bool FindUserByEmail(string email)
        {
            UserModel user = _iUserDao.FindUserByEmail(email);

            if (user == null)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
