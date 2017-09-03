using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HorseSpot.DAL.Models;
using HorseSpot.Models.Models;
using MongoDB.Bson;

namespace HorseSpot.BLL.Converters
{
    /// <summary>
    /// Static class used to map the horse ad database model to domain trasfer objects and vice-versa
    /// </summary>
    public class HorseAdConverter
    {
        /// <summary>
        /// Converts horseDTO ad to horse ad database model
        /// </summary>
        /// <param name="adId">Horse Advertisment Id</param>
        /// <param name="horseAdDTO">Horse ad Advertisment DTO</param>
        /// <returns>Horse ad Database Model</returns>
        public static HorseAd FromHorseAdDTOToHorseAd(ObjectId adId, HorseAdDTO horseAdDTO)
        {
            var horseAd = new HorseAd
            {
                Id = adId,
                HorseName = horseAdDTO.HorseName,
                Abilities = horseAdDTO.Abilities.Select(HorseAbilityConverter.FromHorseAbilityDTOToHorseAbility),
                Address = FromAddressDTOToAddress(horseAdDTO.Address),
                Pedigree = FromPedigreeDTOToPedigree(horseAdDTO.Pedigree),
                Price = horseAdDTO.Price,
                PriceRange = PriceRangeConverter.FromPriceRangeDTOToPriceRange(horseAdDTO.PriceRange),
                Breed = horseAdDTO.Breed,
                Age = horseAdDTO.Age,
                Description = horseAdDTO.Description,
                Gender = GenderConverter.FromGenderDTOToGender(horseAdDTO.Gender),
                HaveCompetionalExperience = horseAdDTO.HaveCompetionalExperience,
                HaveXRays = horseAdDTO.HaveXRays,
                RecomendedRiders = horseAdDTO != null ? horseAdDTO.RecomendedRiders.Select(RecommendedRiderConverter.FromRiderDTOToRider) : null,
                IsSponsorized = horseAdDTO.IsSponsorized,
                VideoLink = horseAdDTO.VideoLink,
                DatePosted = DateTime.UtcNow,
                Title = SetAdTitle(horseAdDTO),
                FavoriteFor = new List<string>(),
                Height = horseAdDTO.HeightInCm,
                ImageIds = new List<string>(),
                IsValidated = false
            };

            return horseAd;
        }

        /// <summary>
        /// Generates the advertisment title
        /// </summary>
        /// <param name="horseAdDTO">Horse Advertisment Model</param>
        /// <returns>Horse Advertisment Title</returns>
        private static string SetAdTitle(HorseAdDTO horseAdDTO)
        {
            var title = horseAdDTO.Age + "y " + horseAdDTO.HorseName;

            return title;
        }

        /// <summary>
        /// Converts PedigreeDTO to pedigree database model
        /// </summary>
        /// <param name="pedigreeDTO">Pedigree domain transfer object</param>
        /// <returns>Pedigree database model</returns>
        private static Pedigree FromPedigreeDTOToPedigree(PedigreeDTO pedigreeDTO)
        {
            if (pedigreeDTO == null)
            {
                return null;
            }

            var pedigree = new Pedigree
            {
                Father = pedigreeDTO.Father,
                Father_Father = pedigreeDTO.Father_Father,
                Father_Mother = pedigreeDTO.Father_Mother,
                Father_Father_Father = pedigreeDTO.Father_Father_Father,
                Father_Father_Mother = pedigreeDTO.Father_Father_Mother,
                Father_Mother_Father = pedigreeDTO.Father_Mother_Father,
                Father_Mother_Mother = pedigreeDTO.Father_Mother_Mother,
                Father_Father_Father_Father = pedigreeDTO.Father_Father_Father_Father,
                Father_Father_Father_Mother = pedigreeDTO.Father_Father_Father_Mother,
                Father_Father_Mother_Father = pedigreeDTO.Father_Father_Mother_Father,
                Father_Father_Mother_Mother = pedigreeDTO.Father_Father_Mother_Mother,
                Father_Mother_Father_Father = pedigreeDTO.Father_Mother_Father_Father,
                Father_Mother_Father_Mother = pedigreeDTO.Father_Mother_Father_Mother,
                Father_Mother_Mother_Father = pedigreeDTO.Father_Mother_Mother_Father,
                Father_Mother_Mother_Mother = pedigreeDTO.Father_Mother_Mother_Mother,
                Mother = pedigreeDTO.Mother,
                Mother_Father = pedigreeDTO.Mother_Father,
                Mother_Mother = pedigreeDTO.Mother_Mother,
                Mother_Father_Father = pedigreeDTO.Mother_Father_Father,
                Mother_Father_Mother = pedigreeDTO.Mother_Father_Mother,
                Mother_Mother_Father = pedigreeDTO.Mother_Mother_Father,
                Mother_Mother_Mother = pedigreeDTO.Mother_Mother_Mother,
                Mother_Father_Father_Father = pedigreeDTO.Mother_Father_Father_Father,
                Mother_Father_Father_Mother = pedigreeDTO.Mother_Father_Father_Mother,
                Mother_Father_Mother_Father = pedigreeDTO.Mother_Father_Mother_Father,
                Mother_Father_Mother_Mother = pedigreeDTO.Mother_Father_Mother_Mother,
                Mother_Mother_Father_Father = pedigreeDTO.Mother_Mother_Father_Father,
                Mother_Mother_Father_Mother = pedigreeDTO.Mother_Mother_Father_Mother,
                Mother_Mother_Mother_Father = pedigreeDTO.Mother_Mother_Mother_Father,
                Mother_Mother_Mother_Mother = pedigreeDTO.Mother_Mother_Mother_Mother
            };

            return pedigree;
        }

        /// <summary>
        /// Converts AddressDTO to address database model
        /// </summary>
        /// <param name="addressDTO">Address domain object model</param>
        /// <returns>Address database model</returns>
        private static Address FromAddressDTOToAddress(AddressDTO addressDTO)
        {
            if (addressDTO == null)
            {
                return null;
            }

            var address = new Address
            {
                Country = addressDTO.Country,
                City = addressDTO.City,
                Street = addressDTO.Street
            };

            return address;
        }

        /// <summary>
        /// Converts horse ad database model to horse DTO
        /// </summary>
        /// <param name="horseAd">Horse ad database model</param>
        /// <returns>HorseAdDTO</returns>
        public static HorseAdDTO FromHorseAdToHorseAdDTO(HorseAd horseAd)
        {
            var horseAdDTO = new HorseAdDTO
            {
                Id = horseAd.Id.ToString(),
                UserId = horseAd.UserId,
                HorseName = horseAd.HorseName,
                Abilities = horseAd.Abilities.Select(HorseAbilityConverter.FromHorseAbilityToHorseAbilityDTO),
                Address = FromAddressToAddressDTO(horseAd.Address),
                Pedigree = FromPedigreeToPedigreeDTO(horseAd.Pedigree),
                Price = horseAd.Price,
                PriceRange = PriceRangeConverter.FromPriceRangeToPriceRangeDTO(horseAd.PriceRange),
                Breed = horseAd.Breed,
                Age = horseAd.Age,
                Description = horseAd.Description,
                Gender = GenderConverter.FromGenderToGenderDTO(horseAd.Gender),
                HaveCompetionalExperience = horseAd.HaveCompetionalExperience,
                HaveXRays = horseAd.HaveXRays,
                ImageIds = horseAd.ImageIds,
                RecomendedRiders = horseAd.RecomendedRiders.Select(RecommendedRiderConverter.FromRiderToRiderDTO),
                IsSponsorized = horseAd.IsSponsorized,
                VideoLink = horseAd.VideoLink,
                DatePosted = horseAd.DatePosted,
                Title = horseAd.Title,
                IsValidated = horseAd.IsValidated,
                HeightInCm = horseAd.Height,
                CountFavoritesFor = horseAd.FavoriteFor.Count,
                Views = horseAd.Views,
                FavoritesFor = horseAd.FavoriteFor
            };

            return horseAdDTO;
        }

        /// <summary>
        /// Converts pedigree database model to pedigree DTO
        /// </summary>
        /// <param name="pedigree">Pedigree database model</param>
        /// <returns>Pedigree DTO</returns>
        private static PedigreeDTO FromPedigreeToPedigreeDTO(Pedigree pedigree)
        {
            if (pedigree == null)
            {
                return null;
            }

            return new PedigreeDTO
            {
                Father = pedigree.Father,
                Father_Father = pedigree.Father_Father,
                Father_Mother = pedigree.Father_Mother,
                Father_Father_Father = pedigree.Father_Father_Father,
                Father_Father_Mother = pedigree.Father_Father_Mother,
                Father_Mother_Father = pedigree.Father_Mother_Father,
                Father_Mother_Mother = pedigree.Father_Mother_Mother,
                Father_Father_Father_Father = pedigree.Father_Father_Father_Father,
                Father_Father_Father_Mother = pedigree.Father_Father_Father_Mother,
                Father_Father_Mother_Father = pedigree.Father_Father_Mother_Father,
                Father_Father_Mother_Mother = pedigree.Father_Father_Mother_Mother,
                Father_Mother_Father_Father = pedigree.Father_Mother_Father_Father,
                Father_Mother_Father_Mother = pedigree.Father_Mother_Father_Mother,
                Father_Mother_Mother_Father = pedigree.Father_Mother_Mother_Father,
                Father_Mother_Mother_Mother = pedigree.Father_Mother_Mother_Mother,
                Mother = pedigree.Mother,
                Mother_Father = pedigree.Mother_Father,
                Mother_Mother = pedigree.Mother_Mother,
                Mother_Father_Father = pedigree.Mother_Father_Father,
                Mother_Father_Mother = pedigree.Mother_Father_Mother,
                Mother_Mother_Father = pedigree.Mother_Mother_Father,
                Mother_Mother_Mother = pedigree.Mother_Mother_Mother,
                Mother_Father_Father_Father = pedigree.Mother_Father_Father_Father,
                Mother_Father_Father_Mother = pedigree.Mother_Father_Father_Mother,
                Mother_Father_Mother_Father = pedigree.Mother_Father_Mother_Father,
                Mother_Father_Mother_Mother = pedigree.Mother_Father_Mother_Mother,
                Mother_Mother_Father_Father = pedigree.Mother_Mother_Father_Father,
                Mother_Mother_Father_Mother = pedigree.Mother_Mother_Father_Mother,
                Mother_Mother_Mother_Father = pedigree.Mother_Mother_Mother_Father,
                Mother_Mother_Mother_Mother = pedigree.Mother_Mother_Mother_Mother
            };
        }

        /// <summary>
        /// Creates the search model dao from search view model
        /// </summary>
        /// <param name="searchModel">Search view model</param>
        /// <returns>Search model dao</returns>
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
                GenderId = searchModel.GenderId,
                PriceRangeId = searchModel.PriceRangeId,
                ToHaveCompetionalExperience = searchModel.ToHaveCompetionalExperience,
                ToHaveVideo = searchModel.ToHaveVideo,
                ToHaveXRays = searchModel.ToHaveXRays,
                Country = searchModel.Country,
                SortAfter = searchModel.SortModel.SortAfter,
                SortDirection = searchModel.SortModel.SortDirection
            };
        }

        /// <summary>
        /// Converts address database model to address DTO
        /// </summary>
        /// <param name="address">Address database model</param>
        /// <returns>Address DTO</returns>
        private static AddressDTO FromAddressToAddressDTO(Address address)
        {
            if (address == null)
            {
                return null;
            }

            var addressDTO = new AddressDTO
            {
                Country = address.Country,
                City = address.City,
                Street = address.Street
            };

            return addressDTO;
        }

        /// <summary>
        /// Generates a Horse Advertisment List Model from horse advertisment database model
        /// </summary>
        /// <param name="horseAd">Horse Advertisment database model</param>
        /// <returns>Horse advertisment list model</returns>
        public static HorseAdListModel FromHorseAdToHorseAdListModel(HorseAd horseAd)
        {
            return new HorseAdListModel
            {
                Id = horseAd.Id.ToString(),
                Title = horseAd.Title,
                ImageId = horseAd.ImageIds.FirstOrDefault(),
                Age = horseAd.Age,
                Breed = horseAd.Breed,
                HorseName = horseAd.HorseName,
                PriceRange = horseAd.PriceRange.PriceRangeValue,
                Price = horseAd.Price.ToString(), 
                IsValidated = horseAd.IsValidated,
                UserId = horseAd.UserId,
                CountFavoritesFor = horseAd.FavoriteFor.Count,
                Views = horseAd.Views,
                Country = horseAd.Address.Country,
                Gender = horseAd.Gender.GenderValue,
                DatePosted = horseAd.DatePosted
            };
        }
    }
}
