using System.Configuration;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Models;
using HorseSpot.Infrastructure.MailService;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Converters
{
    public static class UtilConverter
    {
        public static HorseAbilityDTO FromHorseAbilityToHorseAbilityDTO(HorseAbility horseAbility)
        {
            
            if (horseAbility == null)
            {
                return null;
            }

            var horseAbilityDTO = new HorseAbilityDTO
            {
                Id = horseAbility.Id,
                Ability = horseAbility.Ability
            };

            return horseAbilityDTO;
        }

        public static HorseAbility FromHorseAbilityDTOToHorseAbility(HorseAbilityDTO abilityDTO)
        {
            if (abilityDTO == null)
            {
                return null;
            }

            var horseAbility = new HorseAbility
            {
                Id = abilityDTO.Id,
                Ability = abilityDTO.Ability
            };

            return horseAbility;
        }

        public static PriceRangeDTO FromPriceRangeToPriceRangeDTO(PriceRange priceRange)
        {
            if (priceRange == null)
            {
                return null;
            }

            var priceRangeDTO = new PriceRangeDTO
            {
                Id = priceRange.Id,
                PriceRangeValue = priceRange.PriceRangeValue
            };

            return priceRangeDTO;
        }

        public static PriceRange FromPriceRangeDTOToPriceRange(PriceRangeDTO priceRangeDTO)
        {
            if (priceRangeDTO == null)
            {
                return null;
            }

            var priceRange = new PriceRange
            {
                Id = priceRangeDTO.Id,
                PriceRangeValue = priceRangeDTO.PriceRangeValue
            };

            return priceRange;
        }

        public static RecommendedRiderDTO FromRiderToRiderDTO(RecommendedRider rider)
        {
            return new RecommendedRiderDTO
            {
                Id = rider.Id,
                Rider = rider.Rider
            };
        }

        public static RecommendedRider FromRiderDTOToRider(RecommendedRiderDTO riderDTO)
        {
            return new RecommendedRider
            {
                Id = riderDTO.Id,
                Rider = riderDTO.Rider
            };
        }

        public static ImageModel FromImageDTOToImage(ImageDTO imageDTOObj)
        {
            if (imageDTOObj == null)
            {
                return null;
            }

            return new ImageModel
            {
                Name = imageDTOObj.ImageName,
                IsProfilePic = imageDTOObj.IsProfilePic
            };
        }

        public static ImageDTO FromImageToImageDTO(ImageModel imageObj)
        {
            if (imageObj == null)
            {
                return null;
            }

            return new ImageDTO
            {
                ImageName = imageObj.Name,
                IsProfilePic = imageObj.IsProfilePic,
                ImageId = imageObj.ImageId
            };
        }

        public static SearchModelDao FromSearchModelToSearchModelDao(HorseAdSearchViewModel searchModel)
        {
            return new SearchModelDao
            {
                PageNumber = searchModel.PageNumber,
                AbilityId = searchModel.AbilityId,
                AfterFatherName = searchModel.AfterFatherName,
                MaxAge = searchModel.AgeModel.MaxAge,
                MaxHeight = searchModel.HeightModel.MaxHeight,
                MinAge = searchModel.AgeModel.MinAge,
                MinHeight = searchModel.HeightModel.MinHeight,
                SuitableFor = searchModel.SuitableFor,
                Breed = searchModel.Breed,
                Gender = searchModel.Gender,
                PriceRangeId = searchModel.PriceRangeId,
                RangeSearchList = searchModel.PriceRangeIds,
                ToHaveCompetionalExperience = searchModel.ToHaveCompetionalExperience,
                ToHaveVideo = searchModel.ToHaveVideo,
                ToHaveXRays = searchModel.ToHaveXRays,
                Country = searchModel.Country,
                SortAfter = searchModel.SortModel.SortAfter,
                SortDirection = searchModel.SortModel.SortDirection,
                MinPrice = searchModel.PriceModel.MinPrice,
                MaxPrice = searchModel.PriceModel.MaxPrice
            };
        }

        public static EmailModel FromEmailModelDTOTOEmailModel(EmailModelDTO emailModelDTO)
        {
            return new EmailModel
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = emailModelDTO.Receiver,
                EmailMessage = emailModelDTO.Message,
                EmailSubject = EmailSubjects.EmailFromApplication,
                EmailTemplatePath = EmailTemplatesPath.EmailFromApplicationTemplate,
                SenderName = emailModelDTO.SenderName,
                ReceiverFirstName = emailModelDTO.ReceiverFirstName,
                HorseAdTitle = emailModelDTO.HorseAdTitle
            };
        }

        public static EmailModel FromContactPageEmailModelTOEmailModel(ContactPageEmailModel contactPageEmailModel)
        {
            return new EmailModel
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = ConfigurationManager.AppSettings["AdminEmail"],
                EmailMessage = contactPageEmailModel.Message,
                EmailSubject = EmailSubjects.ContactPageEmailSubject,
                EmailTemplatePath = EmailTemplatesPath.ContactPageEmailTemplate,
                SenderName = contactPageEmailModel.SenderName
            };
        }
    }
}
