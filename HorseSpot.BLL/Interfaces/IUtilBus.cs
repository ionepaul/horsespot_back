using System.Collections.Generic;
using System.Threading.Tasks;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Interfaces
{
    public interface IUtilBus
    {
        Task EmailSendingBetweenUsers(EmailModelDTO emailModelDTO);
        Task ReceiveEmailFromContactPage(ContactPageEmailModel emailModelDTO);
        IEnumerable<RecommendedRiderDTO> GetAllRecommendedRiders();
        IEnumerable<PriceRangeDTO> GetAllPriceRanges();
        IEnumerable<HorseAbilityDTO> GetAllAbilities();
        IEnumerable<string> GetAllCountries();
        Task SendPrivacyPolicyEmail(string email, string fullName);
    }
}
