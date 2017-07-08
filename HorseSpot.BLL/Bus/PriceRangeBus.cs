using HorseSpot.BLL.Converters;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Linq;

namespace HorseSpot.BLL.Bus
{
    public class PriceRangeBus : IPriceRangeBus
    {
        #region Local Variables

        private IPriceRangeDao _iPriceRangeDao;

        #endregion

        #region Constructor
        /// <summary>
        /// PriceRange Constructor
        /// </summary>
        /// <param name="iPriceRangeDao">PriceRange Dao Interface</param>
        public PriceRangeBus(IPriceRangeDao iPriceRangeDao)
        {
            _iPriceRangeDao = iPriceRangeDao;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets all prince ranges
        /// </summary>
        /// <returns>List of PriceRangeDTO</returns>
        public IEnumerable<PriceRangeDTO> GetAll()
        {
            IEnumerable<PriceRange> allPriceRanges = _iPriceRangeDao.GetAll();

            return allPriceRanges.Select(PriceRangeConverter.FromPriceRangeToPriceRangeDTO);
        }

        #endregion
    }
}
