using HorseSpot.BLL.Interfaces;
using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace HorseSpot.Api.Controllers
{
    public class HorseAbilityController : ApiController
    {
        private IHorseAbilityBus _iHorseAbilityBus;

        /// <summary>
        /// HoresAbilityController Constructor
        /// </summary>
        /// <param name="iHorseAbilityBus">HorseAbility Bussines Logic Interface</param>
        public HorseAbilityController(IHorseAbilityBus iHorseAbilityBus)
        {
            _iHorseAbilityBus = iHorseAbilityBus;
        }

        /// <summary>
        /// API Interaface to get all horse abilities(categories) that an advertisment can have 
        /// </summary>
        /// <returns>List of horse abilities</returns>
        [HttpGet]
        [Route("api/abilities")]
        public IEnumerable<HorseAbilityDTO> GetAll()
        {
            return _iHorseAbilityBus.GetAll();
        }
    }
}
