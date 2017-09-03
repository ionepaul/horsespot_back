using HorseSpot.BLL.Interfaces;
using System.Collections.Generic;
using System.Web.Http;

namespace HorseSpot.Api.Controllers
{
    public class CountryController : ApiController
    {
        private ICountryBus _iCountryBus;

        /// <summary>
        /// Country Controller Constructor
        /// </summary>
        /// <param name="iCountryBus">Country Bussines Logic Interface</param>
        public CountryController(ICountryBus iCountryBus)
        {
            _iCountryBus = iCountryBus;
        }

        /// <summary>
        /// API Interaface to get all countries from where advertisments can be added
        /// </summary>
        /// <returns>List of country names</returns>
        [HttpGet]
        [Route("api/countries")]
        public IEnumerable<string> GetAll()
        {
            return _iCountryBus.GetAll();
        }
    }
}
