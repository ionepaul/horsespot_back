using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;

namespace HorseSpot.DAL.Dao
{
    public class GenderDao : AbstractDao<Gender>, IGenderDao
    {
        #region Constructor
        /// <summary>
        /// GenderDao Constructor
        /// </summary>
        /// <param name="dataContext">HorseSpot Relational Data Context</param>
        public GenderDao(HorseSpotDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion
    }
}
