using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HorseSpot.BLL.Converters;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.MailService;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Infrastructure.Validators;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Bus
{
    public class UtilBus : IUtilBus
    {
        #region Local Variables

        private readonly IMailerService _iMailerService;
        private readonly IUtilDao _iUtilDao;

        #endregion

        #region Constructor

        public UtilBus(IMailerService iMailerService, IUtilDao iUtilDao)
        {
            _iMailerService = iMailerService;
            _iUtilDao = iUtilDao;
        }

        #endregion

        #region Public Methods

        public async Task EmailSendingBetweenUsers(EmailModelDTO emailModelDTO)
        {
            if (emailModelDTO == null)
            {
                throw new ValidationException(Resources.InvalidSendEmailRequest);
            }

            ValidationHelper.ValidateModelAttributes<EmailModelDTO>(emailModelDTO);

            ValidateEmailAndMessage(emailModelDTO.Sender, emailModelDTO.Message);

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

            ValidateEmailAndMessage(contactPageEmailModel.Sender, contactPageEmailModel.Message);

            EmailModel emailModel = EmailSendingConverter.FromContactPageEmailModelTOEmailModel(contactPageEmailModel);

            await _iMailerService.SendMail(emailModel);
        }

        public IEnumerable<string> GetAllCountries()
        {
            IEnumerable<Country> countries = _iUtilDao.GetAllCountries();

            return countries.Select(c => c.CountryName);
        }

        public IEnumerable<HorseAbilityDTO> GetAllAbilities()
        {
            IEnumerable<HorseAbility> abilites = _iUtilDao.GetAllAbilities();

            return abilites.Select(HorseAbilityConverter.FromHorseAbilityToHorseAbilityDTO);
        }

        public IEnumerable<PriceRangeDTO> GetAllPriceRanges()
        {
            IEnumerable<PriceRange> priceRanges = _iUtilDao.GetAllPriceRanges();

            return priceRanges.Select(PriceRangeConverter.FromPriceRangeToPriceRangeDTO);
        }

        public IEnumerable<RecommendedRiderDTO> GetAllRecommendedRiders()
        {
            IEnumerable<RecommendedRider> recommendedRiders = _iUtilDao.GetAllRecommendedRiders();

            return recommendedRiders.Select(RecommendedRiderConverter.FromRiderToRiderDTO);
        }

        #endregion

        #region Private Methods

        private void ValidateEmailAndMessage(string email, string message)
        {
            Regex emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

            if (!emailRegex.IsMatch(email))
            {
                throw new ValidationException(Resources.InvalidEmailFormat);
            }

            if (message.Length == 0)
            {
                throw new ValidationException(Resources.InvalidMessageFormat);
            }
        }

        #endregion
    }
}
