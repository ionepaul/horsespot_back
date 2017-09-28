using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
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

namespace HorseSpot.BLL.Bus
{
    public class HorseAdBus : IHorseAdBus
    {
        #region Local Variables

        private IHorseAdDao _iHorseAdDao;
        private IUserDao _iUserDao;
        private IAppointmentDao _iAppointmentDao;
        private IMailerService _iMailerService;
        private IImageDao _iImageDao;

        #endregion

        #region Constructor

        public HorseAdBus(IHorseAdDao iHorseAdDao, IUserDao iAuthDao,IAppointmentDao iAppointmentDao, IMailerService iMailerService, IImageDao iImageDao)
        {
            _iHorseAdDao = iHorseAdDao;
            _iUserDao = iAuthDao;
            _iMailerService = iMailerService;
            _iAppointmentDao = iAppointmentDao;
            _iImageDao = iImageDao;
        }

        #endregion

        #region Public Methods

        public async Task Add(HorseAdDTO horseAdDTO, string userId)
        {
            ValidateHorseAd(horseAdDTO);

            var horseAd = HorseAdConverter.FromHorseAdDTOToHorseAd(horseAdDTO, userId);

            _iHorseAdDao.AddHorse(horseAd);

            await SendEmailToAdmin();
        }
        
        public async Task Update(int id, HorseAdDTO horseAdDTO, string userId)
        {
            ValidateHorseAd(horseAdDTO);

            var horseAd = _iHorseAdDao.GetById(id);

            CheckHorseAdAndUserIdentity(horseAd, userId);
            
            var updatedHorseAd = HorseAdConverter.FromHorseAdDTOToHorseAd(horseAdDTO, horseAd.UserId);

            await _iHorseAdDao.UpdateAsync(updatedHorseAd);
        }

        public async Task Delete(int id, string userId, bool isSold)
        {
            var horseAd = _iHorseAdDao.GetById(id);

            CheckHorseAdAndUserIdentity(horseAd, userId);

            var associatedAppointments = _iAppointmentDao.GetAppointmentsByHorseAdvertismentId(horseAd.Id);

            if (associatedAppointments != null && associatedAppointments.Any())
            {
                foreach (var appointment in associatedAppointments)
                {
                    appointment.IsCanceled = true;

                    _iAppointmentDao.UpdateAppointment(appointment);

                    await SendAppointmentCanceledEmailToInitiator(appointment, horseAd);
                }
            }

            horseAd.IsDeleted = true;
            horseAd.IsSold = isSold;

            await _iHorseAdDao.UpdateAsync(horseAd);
        }

        public async Task Validate(int id)
        {
            var horseAd = _iHorseAdDao.GetById(id);

            CheckIfHorseAdExists(horseAd);

            horseAd.IsValidated = true;

            await _iHorseAdDao.UpdateAsync(horseAd);

            await SendHorseAdValidatedEmail(horseAd);
        }

        public async Task AddToFavorite(int id, string userId)
        {
            var horseAd = _iHorseAdDao.GetById(id);

            CheckIfHorseAdExists(horseAd);

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
                user.FavoriteHorseAds.Add(new UserFavoriteHorseAd
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

        public async Task IncreaseViews(int id)
        {
            var horseAd = _iHorseAdDao.GetById(id);

            CheckIfHorseAdExists(horseAd);

            horseAd.Views += 1;

            await _iHorseAdDao.UpdateAsync(horseAd);
        }

        public HorseAdDTO GetById(int id)
        {
            var horseAd = _iHorseAdDao.GetById(id);

            CheckIfHorseAdExists(horseAd);

            return HorseAdConverter.FromHorseAdToHorseAdDTO(horseAd);
        }
        
        public GetHorseAdListResultsDTO GetAllForAdmin(int pageNumber)
        {
            var allUnvalidatedHorsePosts = _iHorseAdDao.GetAllForAdmin(pageNumber);

            return HorseAdConverter.ConvertHorseListResult(allUnvalidatedHorsePosts);
        }

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

            var matchHorses = _iHorseAdDao.SearchAfter(searchQuery, searchViewModel.PageNumber);

            return HorseAdConverter.ConvertHorseListResult(matchHorses);
        }

        public bool CheckPostOwner(int adId, string userId)
        {
            var horseAd = _iHorseAdDao.GetById(adId);

            CheckIfHorseAdExists(horseAd);

            if (horseAd.UserId == userId)
            {
                return true;
            }

            return false;
        }

        public async Task SaveNewImage(int adId, string imageName, string userId)
        {
            var horseAd = _iHorseAdDao.GetById(adId);

            CheckHorseAdAndUserIdentity(horseAd, userId);

            var image = new ImageModel { Name = imageName, IsProfilePic = false };

            horseAd.Images.Add(image);

            await _iHorseAdDao.UpdateAsync(horseAd);
        }

        public string DeleteImage(int imageId, string userId)
        {
            var image = _iImageDao.GetById(imageId);

            CheckImageAndUserIdentity(image, userId);

            var imageName = image.Name;

            _iImageDao.Delete(image);

            return imageName;
        }

        public void SetHorseAdProfilePicture(int imageId, string userId)
        {
            var image = _iImageDao.GetById(imageId);

            CheckImageAndUserIdentity(image, userId);

            image.HorseAd.Images.Where(img => img.IsProfilePic).FirstOrDefault().IsProfilePic = false;

            image.IsProfilePic = true;

            _iImageDao.Update(image);
        }

        #endregion

        #region Private Methods

        private void ValidateHorseAd(HorseAdDTO horseAdDTO)
        {
            if (horseAdDTO == null)
            {
                throw new ValidationException(Resources.InvalidHorseAdRequest);
            }

            ValidationHelper.ValidateModelAttributes<HorseAdDTO>(horseAdDTO);
            ValidationHelper.ValidateModelAttributes<AddressDTO>(horseAdDTO.Address);

            if (!horseAdDTO.AbilityIds.Any())
            {
                throw new ValidationException(Resources.MustSelectAtLeastOneAbility);
            }

            if (!horseAdDTO.RecomendedRidersIds.Any())
            {
                throw new ValidationException(Resources.MustSelectAtLeastOneRecommendedRider);
            }
        }

        private void CheckHorseAdAndUserIdentity(HorseAd horseAd, string userId)
        {
            CheckIfHorseAdExists(horseAd);

            if (horseAd.UserId != userId)
            {
                throw new ForbiddenException(Resources.ActionRequiresAdditionalRights);
            }
        }

        private void CheckIfHorseAdExists(HorseAd horseAd)
        {
            if (horseAd == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidAdIdentifier);
            }
        }

        private void CheckImageAndUserIdentity(ImageModel image, string userId)
        {
            if (image == null)
            {
                throw new ResourceNotFoundException(Resources.ImageNotFoundInAdImagesList);
            }

            if (image.HorseAd.UserId != userId)
            {
                throw new ForbiddenException(Resources.ActionRequiresAdditionalRights);
            }
        }

        private async Task SendEmailToAdmin()
        {
            EmailModel emailModel = new EmailModel()
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = ConfigurationManager.AppSettings["AdminEmail"],
                ValidateLink = ConfigurationManager.AppSettings["UnvalidatedHorses"],
                EmailSubject = EmailSubjects.PleaseValidateHorseAd,
                EmailTemplatePath = EmailTemplatesPath.ValidateHorseAdTemplate
            };

            await _iMailerService.SendMail(emailModel);
        }

        private async Task SendAppointmentCanceledEmailToInitiator(Appointment appointment, HorseAd horseAd)
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

            await _iMailerService.SendMail(emailModel);
        }

        private async Task SendHorseAdValidatedEmail(HorseAd horseAd)
        {
            var user = _iUserDao.FindUserById(horseAd.UserId);

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

        #endregion
    }
}
