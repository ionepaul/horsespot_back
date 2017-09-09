using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;

namespace HorseSpot.DAL.Dao
{
    public class CountryDao : AbstractDao<Country>, ICountryDao
    {
        #region Constructor
        /// <summary>
        /// CountryDao Constructor
        /// </summary>
        /// <param name="dataContext">HorseSpot Relational Database Context</param>
        public CountryDao(HorseSpotDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion
    }
}
