using HorseSpot.Models.Models;
using HorseSpot.DAL.Models;
using HorseSpot.Infrastructure.Constants;

namespace HorseSpot.BLL.Converters
{
    /// <summary>
    /// Static class used to map the user database model to aplication view model an vice-versa
    /// </summary>
    public static class UserConverter
    {
        /// <summary>
        /// Converts the user view model to database user model
        /// </summary>
        /// <param name="user">User View Model</param>
        /// <returns>User Database Model</returns>
        public static UserModel ConvertUserViewModelToUserModel(UserViewModel user)
        {
            return new UserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                ImagePath = ApplicationConstants.DefaultProfilePhoto,
                NewsletterSubscription = user.NewsletterSubscription,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.Email
            };
        }

        /// <summary>
        /// Converts user database model to user view model
        /// </summary>
        /// <param name="user">User Database Model</param>
        /// <returns>User View Model</returns>
        public static UserViewModel FromUserModelToUserViewModel(UserModel user)
        {
            if (user == null)
            {
                return null;
            }
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                NewsletterSubscription = user.NewsletterSubscription,
                PhoneNumber = user.PhoneNumber
            };

            return userViewModel;
        }

        /// <summary>
        /// Converts database model to user domanin transfer object
        /// </summary>
        /// <param name="user">User database model</param>
        /// <returns>UserDTO</returns>
        public static UserDTO FromUserModelToUserDTO(UserModel user)
        {
            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ImagePath = user.ImagePath,
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}

