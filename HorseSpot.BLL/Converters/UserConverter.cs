using System.Linq;
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
                UserName = user.Email,
                TermsAccepted = user.TermsAccepted,
                DisplayEmail = user.DisplayEmail,
                DisplayPhoneNumber = user.DisplayPhoneNumber
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
                ProfileImage = user.ImagePath,
                TermsAccepted = user.TermsAccepted,
                DisplayEmail = user.DisplayEmail,
                DisplayPhoneNumber = user.DisplayPhoneNumber
            };

            return userViewModel;
        }

        public static UserDTO FromUserModelToUserDTO(UserModel user)
        {
            if (user == null)
            {
                return null;
            }

            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName + " " + user.LastName,
                LastName = user.LastName,
                Email = user.Email,
                ImagePath = user.ImagePath,
                PhoneNumber = user.PhoneNumber,
                DisplayEmail = user.DisplayEmail,
                DisplayPhoneNumber = user.DisplayPhoneNumber
            };
        }

        public static UserFullProfile FromUserModelToUserFullProfile(UserModel user)
        {
            var forSaleQuery = user.HorseAds?.Where(x => !x.IsSold && !x.IsDeleted && x.IsValidated);
            var forSaleReference = user.HorseAds?.Where(x => x.IsSold);
            var favoritesQuery = user.FavoriteHorseAds.Where(x => !x.IsDeleted);

            return new UserFullProfile
            {
                UserId = user.Id,
                FullName = user.FirstName + " " + user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ImagePath = user.ImagePath,
                TotalForSale = forSaleQuery?.Count() ?? 0,
                TotalReferenes = forSaleReference?.Count() ?? 0,
                TotalFavorites = favoritesQuery?.Count() ?? 0,
                FavoriteHorses = favoritesQuery.Select(x => HorseAdConverter.FromHorseAdToHorseAdListModel(x.FavoriteHorseAd)).AsEnumerable(),
                HorsesForSale = forSaleQuery?.OrderByDescending(x => x.DatePosted).Take(3).Select(HorseAdConverter.FromHorseAdToHorseAdListModel),
                ReferenceHorses = forSaleReference?.OrderByDescending(x => x.DatePosted).Take(3).Select(HorseAdConverter.FromHorseAdToHorseAdListModel),
                DisplayPhoneNumber = user.DisplayPhoneNumber,
                DisplayEmail = user.DisplayEmail,
                NewsletterSubscription = user.NewsletterSubscription
            };
        }
    }
}

