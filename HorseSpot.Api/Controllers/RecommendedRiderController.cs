using HorseSpot.BLL.Interfaces;
using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace HorseSpot.Api.Controllers
{
    public class RecommendedRiderController : ApiController
    {
        private IRecommendedRiderBus _iRecommendedRiderBus;

        /// <summary>
        /// RecommendedRide Controller Constructor
        /// </summary>
        /// <param name="iRecommendedRiderBus">Recommended Rider Bussines Logic Interface</param>
        public RecommendedRiderController(IRecommendedRiderBus iRecommendedRiderBus)
        {
            _iRecommendedRiderBus = iRecommendedRiderBus;
        }

        /// <summary>
        /// API Interface to get the recommended riders that can be linked to a horse advertisment
        /// </summary>
        /// <returns>List of recommended riders</returns>
        [HttpGet]
        [Route("api/recommendedriders")]
        public IEnumerable<RecommendedRiderDTO> GetAll()
        {
            return _iRecommendedRiderBus.GetAll();
        }
    }
}
