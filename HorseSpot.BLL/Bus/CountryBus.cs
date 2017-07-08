using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HorseSpot.BLL.Bus
{
    public class CountryBus : ICountryBus
    {
        #region Local Variables

        private ICountryDao _iCountryDao;

        #endregion

        #region Constructor
        /// <summary>
        /// CountryBus Constructor
        /// </summary>
        /// <param name="iCountryDao">ConstructorDao Interface</param>
        public CountryBus(ICountryDao iCountryDao)
        {
            _iCountryDao = iCountryDao;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Get All Countries
        /// </summary>
        /// <returns>List of Country names</returns>
        public IEnumerable<string> GetAll()
        {
            IEnumerable<Country> allEuropeCountries = _iCountryDao.GetAll();

            return allEuropeCountries.Select(c => c.CountryName);
        }

        #endregion
    }
}
