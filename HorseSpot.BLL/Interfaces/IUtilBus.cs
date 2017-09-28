﻿using System.Collections.Generic;
using System.Threading.Tasks;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Interfaces
{
    public interface IUtilBus
    {
        Task SetUserProfilePicture(string path, string id);
        Task EmailSendingBetweenUsers(EmailModelDTO emailModelDTO);
        Task ReceiveEmailFromContactPage(ContactPageEmailModel emailModelDTO);
        void CheckFormat(string path);
        IEnumerable<RecommendedRiderDTO> GetAllRecommendedRiders();
        IEnumerable<PriceRangeDTO> GetAllPriceRanges();
        IEnumerable<HorseAbilityDTO> GetAllAbilities();
        IEnumerable<string> GetAllCountries();
    }
}
