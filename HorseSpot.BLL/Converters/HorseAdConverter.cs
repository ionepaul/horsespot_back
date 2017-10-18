using System;
using System.Collections.Generic;
using System.Linq;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Models;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Converters
{
    public class HorseAdConverter
    {

        #region Public Methods

        public static HorseAd FromHorseAdDTOToHorseAd(HorseAdDTO horseAdDTO, string userId)
        {
            var horseAd = new HorseAd
            {
                HorseName = horseAdDTO.HorseName,
                HorseAbilitesIds = horseAdDTO.AbilityIds,
                Address = FromAddressDTOToAddress(horseAdDTO.Address),
                Pedigree = FromPedigreeDTOToPedigree(horseAdDTO.Pedigree),
                Price = horseAdDTO.Price,
                PriceRangeId = horseAdDTO.PriceRangeId,
                Breed = horseAdDTO.Breed,
                Age = horseAdDTO.Age,
                Description = horseAdDTO.Description,
                Gender = horseAdDTO.Gender,
                HaveCompetionalExperience = horseAdDTO.HaveCompetionalExperience,
                HaveXRays = horseAdDTO.HaveXRays,
                RecommendedRiderIds = horseAdDTO.RecomendedRidersIds,
                IsSponsorized = horseAdDTO.IsSponsorized,
                VideoLink = horseAdDTO.VideoLink,
                DatePosted = DateTime.UtcNow,
                Title = SetAdTitle(horseAdDTO),
                Height = horseAdDTO.HeightInCm,
                Images = horseAdDTO.Images.Select(UtilConverter.FromImageDTOToImage).ToList(),
                IsValidated = false,
                UserId = userId
            };

            return horseAd;
        }

        public static HorseAdDTO FromHorseAdToHorseAdDTO(HorseAd horseAd)
        {
            var horseAdDTO = new HorseAdDTO
            {
                Id = horseAd.Id.ToString(),
                UserId = horseAd.UserId,
                HorseName = horseAd.HorseName,
                Abilities = horseAd.Abilities.Select(UtilConverter.FromHorseAbilityToHorseAbilityDTO),
                Address = FromAddressToAddressDTO(horseAd.Address),
                Pedigree = FromPedigreeToPedigreeDTO(horseAd.Pedigree),
                Price = horseAd.Price,
                PriceRange = UtilConverter.FromPriceRangeToPriceRangeDTO(horseAd.PriceRange),
                Breed = horseAd.Breed,
                Age = horseAd.Age,
                Description = horseAd.Description,
                Gender = horseAd.Gender,
                HaveCompetionalExperience = horseAd.HaveCompetionalExperience,
                HaveXRays = horseAd.HaveXRays,
                Images = horseAd.Images.Select(UtilConverter.FromImageToImageDTO),
                RecomendedRiders = horseAd.RecomendedRiders.Select(UtilConverter.FromRiderToRiderDTO),
                IsSponsorized = horseAd.IsSponsorized,
                VideoLink = horseAd.VideoLink,
                DatePosted = horseAd.DatePosted,
                Title = horseAd.Title,
                IsValidated = horseAd.IsValidated,
                HeightInCm = horseAd.Height,
                CountFavoritesFor = horseAd.FavoriteFor?.Count ?? 0,
                Views = horseAd.Views,
                FavoritesFor = horseAd.FavoriteFor?.Where(x=> !x.IsDeleted).Select(x => x.UserId) ?? new List<string>()
            };

            return horseAdDTO;
        }

        public static string SetAdTitle(HorseAdDTO horseAdDTO)
        {
            var title = horseAdDTO.Age + "y " + horseAdDTO.HorseName;

            return title;
        }

        public static Pedigree FromPedigreeDTOToPedigree(PedigreeDTO pedigreeDTO)
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
                Mother = pedigreeDTO.Mother,
                Mother_Father = pedigreeDTO.Mother_Father,
                Mother_Mother = pedigreeDTO.Mother_Mother,
                Mother_Father_Father = pedigreeDTO.Mother_Father_Father,
                Mother_Father_Mother = pedigreeDTO.Mother_Father_Mother,
                Mother_Mother_Father = pedigreeDTO.Mother_Mother_Father,
                Mother_Mother_Mother = pedigreeDTO.Mother_Mother_Mother
            };

            return pedigree;
        }

        public static HorseAdListModel FromHorseAdToHorseAdListModel(HorseAd horseAd)
        {
            var profilePic = horseAd.Images.Where(img => img.IsProfilePic).Select(img => img.Name).FirstOrDefault();

            return new HorseAdListModel
            {
                Id = horseAd.Id.ToString(),
                Title = horseAd.Title,
                ImageId = profilePic ?? horseAd.Images.Select(img => img.Name).FirstOrDefault(),
                Age = horseAd.Age,
                Breed = horseAd.Breed,
                HorseName = horseAd.HorseName,
                PriceRange = horseAd.PriceRange.PriceRangeValue,
                Price = horseAd.Price.ToString(),
                IsValidated = horseAd.IsValidated,
                UserId = horseAd.UserId,
                CountFavoritesFor = horseAd.FavoriteFor?.Count ?? 0,
                Views = horseAd.Views,
                Country = horseAd.Address.Country,
                Gender = horseAd.Gender,
                DatePosted = horseAd.DatePosted,
                Description = horseAd.Description
            };
        }

        public static GetHorseAdListResultsDTO ConvertHorseListResult(GetHorseAdListResults result)
        {
            var horseAdListResult = new GetHorseAdListResultsDTO()
            {
                TotalCount = result.TotalCount,
                HorseAdList = result.HorseAdList.Select(HorseAdConverter.FromHorseAdToHorseAdListModel)
            };

            return horseAdListResult;
        }

        #endregion

        #region Private Methods

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
                Mother = pedigree.Mother,
                Mother_Father = pedigree.Mother_Father,
                Mother_Mother = pedigree.Mother_Mother,
                Mother_Father_Father = pedigree.Mother_Father_Father,
                Mother_Father_Mother = pedigree.Mother_Father_Mother,
                Mother_Mother_Father = pedigree.Mother_Mother_Father,
                Mother_Mother_Mother = pedigree.Mother_Mother_Mother
            };
        }

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

        #endregion

    }
}
