using HorseSpot.BLL.Converters;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Linq;

namespace HorseSpot.BLL.Bus
{
    public class RecommendedRiderBus : IRecommendedRiderBus
    {
        #region Local Variables

        private IRecommendedRiderDao _iRecommendedRiderDao;

        #endregion

        #region Constructor
        /// <summary>
        /// RecommendedRiderBus Constructor
        /// </summary>
        /// <param name="iRecommendedRiderDao">RecommendedRider Dao Interface</param>
        public RecommendedRiderBus(IRecommendedRiderDao iRecommendedRiderDao)
        {
            _iRecommendedRiderDao = iRecommendedRiderDao;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets all recommended riders
        /// </summary>
        /// <returns>List of RecommendedRiderDTO</returns>
        public IEnumerable<RecommendedRiderDTO> GetAll()
        {
            IEnumerable<RecommendedRider> allRiders = _iRecommendedRiderDao.GetAll();

            return allRiders.Select(RecommendedRiderConverter.FromRiderToRiderDTO);
        }

        #endregion
    }
}
