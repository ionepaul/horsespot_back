using System.Configuration;
using HorseSpot.DAL.Entities;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Converters
{
    public static class UserConverter
    {
        public static UserModel ConvertUserViewModelToUserModel(UserViewModel user)
        {
            return new UserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                ImagePath = ConfigurationManager.AppSettings["DefaultPicName"],
                NewsletterSubscription = user.NewsletterSubscription,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.Email
            };
        }

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
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ImagePath
            };

            return userViewModel;
        }

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

