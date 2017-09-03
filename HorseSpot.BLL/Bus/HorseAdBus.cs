using HorseSpot.BLL.Converters;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Interfaces;
using HorseSpot.DAL.Models;
using HorseSpot.Infrastructure.Constants;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.MailService;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Infrastructure.Validators;
using HorseSpot.Models.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace HorseSpot.BLL.Bus
{
    public class HorseAdBus : IHorseAdBus
    {
        #region Local Variables

        private IHorseAdDao _iHorseAdDao;
        private IUserDao _iUserDao;
        private IGenderDao _iGenderDao;
        private IPriceRangeDao _iPriceRangeDao;
        private IHorseAbilityDao _iHorseAbilityDao;
        private IRecommendedRiderDao _iRecommendedRiderDao;
        private IAppointmentDao _iAppointmentDao;
        private IMailerService _iMailerService;

        #endregion

        #region Constructor

        /// <summary>
        /// HorseBus Constructor
        /// </summary>
        /// <param name="iHorseAdDao">HorseAdDao Interface</param>
        /// <param name="iAuthDao">AuthDao Interface</param>
        /// <param name="iGenderDao">GenderDao Interface</param>
        /// <param name="iPriceRangeDao">PriceRangeDao Interface</param>
        /// <param name="iHorseAbilityDao">HorseAbilityDao Interface</param>
        /// <param name="iRecommendedRiderDao">RecommendedRiderDao Interface</param>
        /// <param name="iAppointmentDao">AppointmentDao Interface</param>
        /// <param name="iMailerService">MailerService Interface</param>
        public HorseAdBus(IHorseAdDao iHorseAdDao, IUserDao iAuthDao, IGenderDao iGenderDao,
            IPriceRangeDao iPriceRangeDao, IHorseAbilityDao iHorseAbilityDao, IRecommendedRiderDao iRecommendedRiderDao, 
            IAppointmentDao iAppointmentDao, IMailerService iMailerService)
        {
            _iHorseAdDao = iHorseAdDao;
            _iUserDao = iAuthDao;
            _iGenderDao = iGenderDao;
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
            var validatedHorseAd = ValidateAndSetHorseAd(horseAdDTO);

            HorseAd horseAd = HorseAdConverter.FromHorseAdDTOToHorseAd(new ObjectId(), validatedHorseAd);
            horseAd.UserId = userId;

            _iHorseAdDao.Insert(horseAd);

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
        public void Update(string id, HorseAdDTO horseAdDTO, string userId)
        {
            var validatedHorseAd = ValidateAndSetHorseAd(horseAdDTO);

            var horseAd = _iHorseAdDao.GetById(id);

            if (horseAd == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidAdIdentifier);
            }

            if (horseAd.UserId != userId)
            {
                throw new ForbiddenException(Resources.ActionRequiresAdditionalRights);
            }
            
            var updatedHorseAd = HorseAdConverter.FromHorseAdDTOToHorseAd(horseAd.Id, validatedHorseAd);

            updatedHorseAd.UserId = horseAd.UserId;
            updatedHorseAd.FavoriteFor = horseAd.FavoriteFor;
            updatedHorseAd.DatePosted = horseAd.DatePosted;
            updatedHorseAd.IsValidated = horseAd.IsValidated;
            updatedHorseAd.ImageIds = horseAd.ImageIds;
            updatedHorseAd.Views = horseAd.Views;

            _iHorseAdDao.Update(updatedHorseAd);
        }

        /// <summary>
        /// Delete a horse advertisment and all the associated appointments, sends email to user involved in appointment
        /// </summary>
        /// <param name="id">Horse advertisment id</param>
        /// <param name="userId">The id of the user who tries to delete the post</param>
        /// <returns>Task or exception</returns>
        public async Task Delete(string id, string userId)
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

            var associatedAppointments = _iAppointmentDao.GetAppointmentsByHorseAdvertismentId(horseAd.Id.ToString());

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

                    _iAppointmentDao.Delete(appointment);
                    await _iMailerService.SendMail(emailModel);
                }
            }
                     
            _iHorseAdDao.Delete(horseAd);
        }

        /// <summary>
        /// Validates an advertisment and sends email notification to the post owner
        /// </summary>
        /// <param name="id">Horse Advertisment id</param>
        /// <returns>Task</returns>
        public async Task Validate(string id)
        {
            _iHorseAdDao.Validate(new ObjectId(id));

            var horseAd = _iHorseAdDao.GetById(id);
            var user = _iUserDao.FindUserById(horseAd.UserId);

            EmailModel emailModel = new EmailModel()
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = user.Email,
                ReceiverFirstName = user.FirstName,
                ReceiverLastName = user.LastName,
                HorseAdLink = ConfigurationManager.AppSettings["HorseAdLink"] + horseAd.Id.ToString() + "/" + horseAd.Abilities.First().Ability.ToLowerInvariant() + "/" + horseAd.Age + "y" + "-" + horseAd.HorseName + "-" + horseAd.Gender.GenderValue.ToLowerInvariant(),
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
        public void AddToFavorite(string id, string userId)
        {
            var horseAd = _iHorseAdDao.GetById(id);

            if (horseAd == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidAdIdentifier);
            }

            var favoritesList = horseAd.FavoriteFor;

            if (!favoritesList.Contains(userId))
            {
                favoritesList.Add(userId);
            }
            else
            {
                favoritesList.Remove(userId);
            }

            _iHorseAdDao.UpdateFavoritesList(new ObjectId(id), favoritesList);
        }

        /// <summary>
        /// Increase views for an advertisment
        /// </summary>
        /// <param name="id">Horse Advertisment id</param>
        /// <returns></returns>
        public async Task IncreaseViews(string id)
        {
            var horseAd = _iHorseAdDao.GetById(id);

            if (horseAd == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidAdIdentifier);
            }

            var views = horseAd.Views + 1;

            await _iHorseAdDao.IncreaseViews(new ObjectId(id), views);
        }

        /// <summary>
        /// Gets a horse advertisment by id
        /// </summary>
        /// <param name="id">Horse advertisment id</param>
        /// <returns>Horse advertisment model</returns>
        public HorseAdDTO GetById(string id)
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
        /// Gets the list of user's posted advertisments by page number
        /// </summary>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="userId">User Id</param>
        /// <returns>Model containing total number of posts owned and list of posts</returns>
        public GetHorseAdListResultsDTO GetAllForUser(int pageNumber, string userId)
        {
            var usersHorseAds = _iHorseAdDao.GetPostedAdsFor(userId, pageNumber);

            var results = new GetHorseAdListResultsDTO();
            results.TotalCount = usersHorseAds.TotalCount;
            results.HorseAdList = usersHorseAds.HorseAdList.Select(HorseAdConverter.FromHorseAdToHorseAdListModel);

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
        public bool CheckPostOwner(string adId, string userId)
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates and sets a horse advertisment model
        /// </summary>
        /// <param name="horseAdDTO">Horse Advertisment Model</param>
        /// <returns>Horse Advertisment Model</returns>
        private HorseAdDTO ValidateAndSetHorseAd(HorseAdDTO horseAdDTO)
        {
            if (horseAdDTO == null)
            {
                throw new ValidationException(Resources.InvalidHorseAdRequest);
            }

            ValidationHelper.ValidateModelAttributes<HorseAdDTO>(horseAdDTO);
            ValidationHelper.ValidateModelAttributes<AddressDTO>(horseAdDTO.Address);
            ValidationHelper.ValidateModelAttributes<PriceRangeDTO>(horseAdDTO.PriceRange);

            var priceRange = _iPriceRangeDao.GetById(horseAdDTO.PriceRange.Id);

            if (priceRange == null)
            {
                throw new ValidationException(Resources.InvalidPriceRangeIdentifier);
            }

            horseAdDTO.PriceRange.PriceRangeValue = priceRange.PriceRangeValue;

            var gender = _iGenderDao.GetById(horseAdDTO.Gender.Id);

            if (gender == null)
            {
                throw new ValidationException(Resources.InvalidHorseGenderIdentifier);
            }

            horseAdDTO.Gender.Gender = gender.GenderValue;

            if (!horseAdDTO.Abilities.Any())
            {
                throw new ValidationException(Resources.MustSelectAtLeastOneAbility);
            }

            foreach (var ability in horseAdDTO.Abilities)
            {
                var dbAbility = _iHorseAbilityDao.GetById(ability.Id);

                if (dbAbility == null)
                {
                    throw new ValidationException(Resources.InvalidAbilityIdentifier);
                }

                ability.Ability = dbAbility.Ability;
            }

            if (horseAdDTO.RecomendedRiders.Any())
            {
                foreach(var rider in horseAdDTO.RecomendedRiders)
                {
                    var dbRider = _iRecommendedRiderDao.GetById(rider.Id);

                    if (dbRider == null)
                    {
                        throw new ValidationException(Resources.InvalidRecommendedRiderIdentifier);
                    }

                    rider.Rider = dbRider.Rider;
                }
            }

            return horseAdDTO;
        }

        #endregion
    }
}
