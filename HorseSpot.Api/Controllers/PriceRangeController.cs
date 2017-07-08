using HorseSpot.BLL.Interfaces;
using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace HorseSpot.Api.Controllers
{
    public class PriceRangeController : ApiController
    {
        private IPriceRangeBus _iPriceRangeBus;

        /// <summary>
        /// PriceRangeController Constructor
        /// </summary>
        /// <param name="iPriceRangeBus">PriceRange Bussines Logic Interface</param>
        public PriceRangeController(IPriceRangeBus iPriceRangeBus)
        {
            _iPriceRangeBus = iPriceRangeBus;
        }

        /// <summary>
        /// API Interface to get all price ranges available
        /// </summary>
        /// <returns>List of price ranges</returns>
        [HttpGet]
        [Route("api/priceranges")]
        public IEnumerable<PriceRangeDTO> GetAll()
        {
            return _iPriceRangeBus.GetAll();
        }
    }
}
