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
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HorseSpot.BLL.Bus
{
    public class HorseAdBus : IHorseAdBus
    {
        #region Local Variables

        private IHorseAdDao _iHorseAdDao;
        private IUserDao _iUserDao;
        private IPriceRangeDao _iPriceRangeDao;
        private IHorseAbilityDao _iHorseAbilityDao;
        private IRecommendedRiderDao _iRecommendedRiderDao;
        private IAppointmentDao _iAppointmentDao;
        private IMailerService _iMailerService;

        #endregion

        #region Constructor

        public HorseAdBus(IHorseAdDao iHorseAdDao, IUserDao iAuthDao,
            IPriceRangeDao iPriceRangeDao, IHorseAbilityDao iHorseAbilityDao, IRecommendedRiderDao iRecommendedRiderDao, 
            IAppointmentDao iAppointmentDao, IMailerService iMailerService)
        {
            _iHorseAdDao = iHorseAdDao;
            _iUserDao = iAuthDao;
            _iPriceRangeDao = iPriceRangeDao;
            _iHorseAbilityDao = iHorseAbilityDao;
            _iRecommendedRiderDao = iRecommendedRiderDao;
            _iMailerService = iMailerService;
            _iAppointmentDao = iAppointmentDao;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates and add a new horse addvertisment, sends email to admin in order to validate it
        /// </summary>
        /// <param name="horseAdDTO">Horse Advertisment Model</param>
        /// <param name="userId">The id of the user who post it</param>
        /// <returns>Horse Advertisment Id or exception</returns>
        public async Task<string> Add(HorseAdDTO horseAdDTO, string userId)
        {
            var validatedHorseAd = ValidateHorseAd(horseAdDTO);

            HorseAd horseAd = HorseAdConverter.FromHorseAdDTOToHorseAd(validatedHorseAd, userId);

            try
            {
                _iHorseAdDao.AddHorse(horseAd);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            EmailModel emailModel = new EmailModel()
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = ConfigurationManager.AppSettings["AdminEmail"],
                ValidateLink = ConfigurationManager.AppSettings["UnvalidatedHorses"],
                EmailSubject = EmailSubjects.PleaseValidateHorseAd,
                EmailTemplatePath = EmailTemplatesPath.ValidateHorseAdTemplate
            };

            await _iMailerService.SendMail(emailModel);

            return horseAd.Id.ToString();
        }
        
        /// <summary>
        /// Validates and updates a horse advertisment
        /// </summary>
        /// <param name="id">Horse Advertisment Id</param>
        /// <param name="horseAdDTO">Horse Advertisment Model</param>
        /// <param name="userId">The id of user who tries to update the post</param>
        public async Task Update(int id, HorseAdDTO horseAdDTO, string userId)
        {
            var validatedHorseAd = ValidateHorseAd(horseAdDTO);

            var horseAd = _iHorseAdDao.GetById(id);

            if (horseAd == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidAdIdentifier);
            }

            if (horseAd.UserId != userId)
            {
                throw new ForbiddenException(Resources.ActionRequiresAdditionalRights);
            }
            
            var updatedHorseAd = HorseAdConverter.FromHorseAdDTOToHorseAd(validatedHorseAd, horseAd.UserId);

            await _iHorseAdDao.UpdateAsync(updatedHorseAd);
        }

        /// <summary>
        /// Delete a horse advertisment and all the associated appointments, sends email to user involved in appointment
        /// </summary>
        /// <param name="id">Horse advertisment id</param>
        /// <param name="userId">The id of the user who tries to delete the post</param>
        /// <returns>Task or exception</returns>
        public async Task Delete(int id, string userId, bool isSold)
        {
            var horseAd = _iHorseAdDao.GetById(id);

            if (horseAd == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidAdIdentifier);
            }

            if (horseAd.UserId != userId)
            {
                throw new ForbiddenException(Resources.ActionRequiresAdditionalRights);
            }

            var associatedAppointments = _iAppointmentDao.GetAppointmentsByHorseAdvertismentId(horseAd.Id);

            if (associatedAppointments.Count() != 0)
            {
                foreach (var appointment in associatedAppointments)
                {
                    var initiator = _iUserDao.FindUserById(appointment.InitiatorId);

                    EmailModel emailModel = new EmailModel()
                    {
                        Sender = ConfigurationManager.AppSettings["AdminEmail"],
                        Receiver = initiator.Email,
                        ReceiverFirstName = initiator.FirstName,
                        ReceiverLastName = initiator.LastName,
                        EmailSubject = EmailSubjects.AppointmentCanceled,
                        EmailTemplatePath = EmailTemplatesPath.HorseAdDeleted,
                        HorseAdTitle = horseAd.Title
                    };

                    appointment.IsCanceled = true;
                    _iAppointmentDao.UpdateAppointment(appointment);
                    await _iMailerService.SendMail(emailModel);
                }
            }

            horseAd.IsDeleted = true;
            horseAd.IsSold = isSold;

            await _iHorseAdDao.UpdateAsync(horseAd);
        }

        /// <summary>
        /// Validates an advertisment and sends email notification to the post owner
        /// </summary>
        /// <param name="id">Horse Advertisment id</param>
        /// <returns>Task</returns>
        public async Task Validate(int id)
        {
            var horseAd = _iHorseAdDao.GetById(id);
            var user = _iUserDao.FindUserById(horseAd.UserId);

            horseAd.IsValidated = true;
            await _iHorseAdDao.UpdateAsync(horseAd);

            EmailModel emailModel = new EmailModel()
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = user.Email,
                ReceiverFirstName = user.FirstName,
                ReceiverLastName = user.LastName,
                HorseAdLink = ConfigurationManager.AppSettings["HorseAdLink"] + horseAd.Id + "/" + horseAd.Abilities.First().Ability.ToLowerInvariant() + "/" + horseAd.Age + "y" + "-" + horseAd.HorseName + "-" + horseAd.Gender.ToLowerInvariant(),
                EmailSubject = EmailSubjects.ValidationSucceded,
                EmailTemplatePath = EmailTemplatesPath.ValidationSuccededTemplate
            };

            emailModel.HorseAdLink = emailModel.HorseAdLink.Replace(" ", "");

            await _iMailerService.SendMail(emailModel);
        }

        /// <summary>
        /// Adds a horse advertisment to user's wish list
        /// </summary>
        /// <param name="id">Horse advertisment id</param>
        /// <param name="userId">User id</param>
        public async Task AddToFavorite(int id, string userId)
        {
            var horseAd = _iHorseAdDao.GetById(id);

            if (horseAd == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidAdIdentifier);
            }

            var user = _iUserDao.FindUserById(userId);

            if (user == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidUserIdentifier);
            }

            var favoritesList = user.FavoriteHorseAds.Where(x => x.HorseAdId == id);

            if (favoritesList != null && favoritesList.Any())
            {
                favoritesList.First().IsDeleted = !favoritesList.First().IsDeleted;
            }
            else if (user.FavoriteHorseAds != null)
            {
                user.FavoriteHorseAds?.Add(new UserFavoriteHorseAd
                {
                    UserId = userId,
                    HorseAdId = id,
                    IsDeleted = false
                });
            } else
            {
                user.FavoriteHorseAds = new List<UserFavoriteHorseAd>();
                user.FavoriteHorseAds.Add(new UserFavoriteHorseAd
                {
                    UserId = userId,
                    HorseAdId = id,
                    IsDeleted = false
                });
            }

            var updatedUser = await _iUserDao.UpdateUser(user);
        }

        /// <summary>
        /// Increase views for an advertisment
        /// </summary>
        /// <param name="id">Horse Advertisment id</param>
        /// <returns></returns>
        public async Task IncreaseViews(int id)
        {
            var horseAd = _iHorseAdDao.GetById(id);

            if (horseAd == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidAdIdentifier);
            }

            horseAd.Views += 1;

            await _iHorseAdDao.UpdateAsync(horseAd);
        }

        /// <summary>
        /// Gets a horse advertisment by id
        /// </summary>
        /// <param name="id">Horse advertisment id</param>
        /// <returns>Horse advertisment model</returns>
        public HorseAdDTO GetById(int id)
        {
            var horseAd = _iHorseAdDao.GetById(id);

            if (horseAd == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidAdIdentifier);
            }

            return HorseAdConverter.FromHorseAdToHorseAdDTO(horseAd);
        }
        
        /// <summary>
        /// Get list of unvalidated horse advertisments by page number
        /// </summary>
        /// <param name="pageNumber">Page Number</param>
        /// <returns>Model containing of total number of unvalidated posts and list of unvalidated posts</returns>
        public GetHorseAdListResultsDTO GetAllForAdmin(int pageNumber)
        {
            var allUnvalidatedHorsePosts = _iHorseAdDao.GetAllForAdmin(pageNumber);

            var results = new GetHorseAdListResultsDTO();
            results.TotalCount = allUnvalidatedHorsePosts.TotalCount;
            results.HorseAdList = allUnvalidatedHorsePosts.HorseAdList.Select(HorseAdConverter.FromHorseAdToHorseAdListModel);

            return results;
        }

        /// <summary>
        /// Gets the wish list for an user by page number
        /// </summary>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="userId">User Id</param>
        /// <returns>Model containinin total number of wish list's ads and list of ads</returns>
        public GetHorseAdListResultsDTO GetUserFavorites(int pageNumber, string userId)
        {            
            var usersFavorites = _iHorseAdDao.GetFavoritesFor(userId, pageNumber);

            var results = new GetHorseAdListResultsDTO();
            results.TotalCount = usersFavorites.TotalCount;
            results.HorseAdList = usersFavorites.HorseAdList.Select(HorseAdConverter.FromHorseAdToHorseAdListModel);

            return results;
        }

        /// <summary>
        /// Creates the search model and gets the horse ads that match the search criteria
        /// </summary>
        /// <param name="searchViewModel">SerachModel containing search criteria</param>
        /// <returns>Model containing total number of ads and list of ads that match by page number</returns>
        public GetHorseAdListResultsDTO SearchHorses(HorseAdSearchViewModel searchViewModel)
        { 
            SearchModelDao searchModelDao = HorseAdConverter.FromSearchModelToSearchModelDao(searchViewModel);

            if (searchModelDao.SortAfter != ApplicationConstants.SortAge && searchModelDao.SortAfter != ApplicationConstants.SortDatePosted 
                && searchModelDao.SortAfter != ApplicationConstants.SortHeight && searchModelDao.SortAfter != ApplicationConstants.SortPrice 
                && searchModelDao.SortAfter != ApplicationConstants.SortViews)
            {
                searchModelDao.SortAfter = ApplicationConstants.SortDatePosted;
            }

            if (searchModelDao.SortDirection != 1 && searchModelDao.SortDirection != 0)
            {
                searchModelDao.SortDirection = 1;
            }

            SearchHorseDao searchQuery = new SearchHorseDao(searchModelDao);

            var results = new GetHorseAdListResultsDTO();

            var horses = _iHorseAdDao.SearchAfter(searchQuery, searchViewModel.PageNumber);

            results.TotalCount = horses.TotalCount;
            results.HorseAdList = horses.HorseAdList.Select(HorseAdConverter.FromHorseAdToHorseAdListModel);

            return results;
        }

        /// <summary>
        /// Check if user is owner of the advertisment
        /// </summary>
        /// <param name="adId">Horse Advertisment Id</param>
        /// <param name="userId">User Id</param>
        /// <returns>True/False</returns>
        public bool CheckPostOwner(int adId, string userId)
        {
            var horseAd = _iHorseAdDao.GetById(adId);

            if (horseAd == null)
            {
                return false;
            }

            if (horseAd.UserId == userId)
            {
                return true;
            }

            return false;
        }

        public async Task SaveNewImage(int adId, string imageName, string userId)
        {
            var horseAd = _iHorseAdDao.GetById(adId);

            if (horseAd == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidAdIdentifier);
            }

            if (horseAd.UserId != userId)
            {
                throw new ForbiddenException(Resources.ActionRequiresAdditionalRights);
            }

            var image = new ImageModel { Name = imageName, IsProfilePic = false };
            horseAd.Images.Add(image);

            await _iHorseAdDao.UpdateAsync(horseAd);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates and sets a horse advertisment model
        /// </summary>
        /// <param name="horseAdDTO">Horse Advertisment Model</param>
        /// <returns>Horse Advertisment Model</returns>
        private HorseAdDTO ValidateHorseAd(HorseAdDTO horseAdDTO)
        {
            if (horseAdDTO == null)
            {
                throw new ValidationException(Resources.InvalidHorseAdRequest);
            }

            ValidationHelper.ValidateModelAttributes<HorseAdDTO>(horseAdDTO);
            ValidationHelper.ValidateModelAttributes<AddressDTO>(horseAdDTO.Address);

            var priceRange = _iPriceRangeDao.GetById(horseAdDTO.PriceRangeId);

            if (priceRange == null)
            {
                throw new ValidationException(Resources.InvalidPriceRangeIdentifier);
            }

            if (!horseAdDTO.AbilityIds.Any())
            {
                throw new ValidationException(Resources.MustSelectAtLeastOneAbility);
            }

            if (!horseAdDTO.RecomendedRidersIds.Any())
            {
                throw new ValidationException(Resources.MustSelectAtLeastOneRecommendedRider);
            }

            return horseAdDTO;
        }

        #endregion
    }
}
