using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Models.Enums;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HorseSpot.Models.Models;
using HorseSpot.Infrastructure.Validators;
using HorseSpot.BLL.Converters;
using HorseSpot.Infrastructure.MailService;
using System.Text.RegularExpressions;
using HorseSpot.Infrastructure.Constants;
using HorseSpot.DAL.Entities;

namespace HorseSpot.BLL.Bus
{
    public class UtilBus : IUtilBus
    {
        #region Local Variables

        private readonly IImageDao _iImageDao;
        private readonly IUserDao _iUserDao;
        private readonly IHorseAdDao _iHorseAdDao;
        private readonly IMailerService _iMailerService;
        private readonly ICountryDao _iCountryDao;
        private readonly IRecommendedRiderDao _iRecommendedRiderDao;
        private readonly IHorseAbilityDao _iHorseAbilityDao;
        private readonly IPriceRangeDao _iPriceRangeDao;
        #endregion

        #region Constructor

        public UtilBus(IImageDao iImageDao, IHorseAdDao iHorseAdDao, IUserDao iUserDao, IMailerService iMailerService, ICountryDao iCountryDao, IRecommendedRiderDao iRecommendedRiderDao, IHorseAbilityDao iHorseAbilityDao, IPriceRangeDao iPriceRangeDao)
        {
            _iImageDao = iImageDao;
            _iHorseAdDao = iHorseAdDao;
            _iUserDao = iUserDao;
            _iMailerService = iMailerService;
            _iCountryDao = iCountryDao;
            _iRecommendedRiderDao = iRecommendedRiderDao;
            _iHorseAbilityDao = iHorseAbilityDao;
            _iPriceRangeDao = iPriceRangeDao;
        }

        #endregion

        #region Public Methods

        public string DeleteImage(int imageId, string userId)
        {
            var image = _iImageDao.GetById(imageId);
            
            if (image == null)
            {
                throw new ResourceNotFoundException(Resources.ImageNotFoundInAdImagesList);
            }

            if (image.HorseAd.UserId != userId)
            {
                throw new ForbiddenException(Resources.ActionRequiresAdditionalRights);
            }

            var imageName = image.Name;
            _iImageDao.Delete(image);

            return imageName;
        }

        public void SetHorseAdProfilePicture(int imageId, string userId)
        {
            var image = _iImageDao.GetById(imageId);

            if (image == null)
            {
                throw new ResourceNotFoundException(Resources.ImageNotFoundInAdImagesList);
            }

            if (image.HorseAd.UserId != userId)
            {
                throw new ForbiddenException(Resources.ActionRequiresAdditionalRights);
            }

            image.HorseAd.Images.Where(img => img.IsProfilePic).FirstOrDefault().IsProfilePic = false;
            image.IsProfilePic = true;

            _iImageDao.Update(image);
        }

        public async Task SetUserProfilePicture(string path, string id)
        {
            var user = _iUserDao.FindUserById(id);

            if (user == null)
            {
                throw new ValidationException(Resources.InvalidUserIdentifier);
            }

            if (user.ImagePath == ApplicationConstants.DefaultProfilePhoto)
            {
                CheckFormat(path);
                user.ImagePath = path;
            }
            else
            {
                CheckFormat(path);

                var currentImagePath = user.ImagePath;
                if (File.Exists(currentImagePath))
                {
                    File.Delete(currentImagePath);
                }

                user.ImagePath = path;
            }

            await _iUserDao.UpdateUser(user);
        }

        public async Task EmailSendingBetweenUsers(EmailModelDTO emailModelDTO)
        {
            if (emailModelDTO == null)
            {
                throw new ValidationException(Resources.InvalidSendEmailRequest);
            }

            ValidationHelper.ValidateModelAttributes<EmailModelDTO>(emailModelDTO);

            Regex emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

            if (!emailRegex.IsMatch(emailModelDTO.Sender))
            {
                throw new ValidationException(Resources.InvalidEmailFormat);
            }

            if (emailModelDTO.Message.Length == 0)
            {
                throw new ValidationException(Resources.InvalidMessageFormat);
            }

            EmailModel emailModel = EmailSendingConverter.FromEmailModelDTOTOEmailModel(emailModelDTO);

            await _iMailerService.SendMail(emailModel);
        }

        public async Task ReceiveEmailFromContactPage(ContactPageEmailModel contactPageEmailModel)
        {
            if (contactPageEmailModel == null)
            {
                throw new ValidationException(Resources.InvalidSendEmailRequest);
            }

            ValidationHelper.ValidateModelAttributes<ContactPageEmailModel>(contactPageEmailModel);

            Regex emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

            if (!emailRegex.IsMatch(contactPageEmailModel.Sender))
            {
                throw new ValidationException(Resources.InvalidEmailFormat);
            }

            if (contactPageEmailModel.Message.Length == 0)
            {
                throw new ValidationException(Resources.InvalidMessageFormat);
            }

            EmailModel emailModel = EmailSendingConverter.FromContactPageEmailModelTOEmailModel(contactPageEmailModel);

            await _iMailerService.SendMail(emailModel);
        }

        public void CheckFormat(string path)
        {
            var extension = Path.GetExtension(path).Replace(".", "");

            if (!Enum.IsDefined(typeof(SupportedImageExtensionEnum), extension.ToUpper()))
            {
                throw new ValidationException(Resources.InvalidPictureFormat);
            }
        }

        public IEnumerable<string> GetAllCountries()
        {
            IEnumerable<Country> countries = _iCountryDao.GetAll();

            return countries.Select(c => c.CountryName);
        }

        public IEnumerable<HorseAbilityDTO> GetAllAbilities()
        {
            IEnumerable<HorseAbility> abilites = _iHorseAbilityDao.GetAll();

            return abilites.Select(HorseAbilityConverter.FromHorseAbilityToHorseAbilityDTO);
        }

        public IEnumerable<PriceRangeDTO> GetAllPriceRanges()
        {
            IEnumerable<PriceRange> priceRanges = _iPriceRangeDao.GetAll();

            return priceRanges.Select(PriceRangeConverter.FromPriceRangeToPriceRangeDTO);
        }

        public IEnumerable<RecommendedRiderDTO> GetAllRecommendedRiders()
        {
            IEnumerable<RecommendedRider> recommendedRiders = _iRecommendedRiderDao.GetAll();

            return recommendedRiders.Select(RecommendedRiderConverter.FromRiderToRiderDTO);
        }

        #endregion
    }
}
