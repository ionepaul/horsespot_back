﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using HorseSpot.BLL.Interfaces;
using HorseSpot.Models.Models;

namespace HorseSpot.Api.Controllers
{
    public class UtilsController : ApiController
    {
        private readonly IUtilBus _iUtilBus;
        private readonly IUserBus _iUserBus;

        public UtilsController(IUtilBus iUtilBus, IUserBus iUserBus)
        {
            _iUtilBus = iUtilBus;
            _iUserBus = iUserBus;
        }

        #region HttpGet

        [HttpGet]
        [Route("api/countries")]
        public IEnumerable<string> GetAllCountries()
        {
            return _iUtilBus.GetAllCountries();
        }

        [HttpGet]
        [Route("api/abilities")]
        public IEnumerable<HorseAbilityDTO> GetAllAbilities()
        {
            return _iUtilBus.GetAllAbilities();
        }

        [HttpGet]
        [Route("api/priceranges")]
        public IEnumerable<PriceRangeDTO> GetAllPriceRanges()
        {
            return _iUtilBus.GetAllPriceRanges();
        }

        [HttpGet]
        [Route("api/recommendedriders")]
        public IEnumerable<RecommendedRiderDTO> GetAllRecommendedRiders()
        {
            return _iUtilBus.GetAllRecommendedRiders();
        }

        #endregion

        #region HttpPost

        [HttpPost]
        [Route("api/sendemail")]
        public async Task SendMail([FromBody] EmailModelDTO emailModelDTO)
        {
            await _iUtilBus.EmailSendingBetweenUsers(emailModelDTO);
        }

        [HttpPost]
        [Route("api/sendemail/all")]
        public async Task SendEmailToAllUsers()
        {
            var users = _iUserBus.GetAllUsers().Skip(49);

            try
            {
                foreach (var user in users)
                {
                    var fullName = user.FirstName + " " + user.LastName;
                    await _iUtilBus.SendPrivacyPolicyEmail(user.Email, fullName);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

        }

        [HttpPost]
        [Route("api/contactformemail")]
        public async Task ReceiveEmailFromContact([FromBody] ContactPageEmailModel contactPageEmailModel)
        {
            await _iUtilBus.ReceiveEmailFromContactPage(contactPageEmailModel);
        }

        #endregion

    }
}
