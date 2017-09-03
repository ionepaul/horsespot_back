using HorseSpot.BLL.Interfaces;
using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace HorseSpot.Api.Controllers
{
    public class HorseGenderController : ApiController
    {
        private IGenderBus _iGenderBus;

        /// <summary>
        /// HorseGenderController
        /// </summary>
        /// <param name="iGenderBus">Gender Bussines Logic Interface</param>
        public HorseGenderController(IGenderBus iGenderBus)
        {
            _iGenderBus = iGenderBus;
        }

        /// <summary>
        /// API Interaface to get all genders
        /// </summary>
        /// <returns>List of genders</returns>
        [HttpGet]
        [Route("api/genders")]
        public IEnumerable<GenderDTO> GetAll()
        {
            return _iGenderBus.GetAll();
        }
    }
}
