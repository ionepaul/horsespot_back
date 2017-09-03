using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;

namespace HorseSpot.DAL.Dao
{
    public class RecommendedRiderDao : AbstractDao<RecommendedRider>, IRecommendedRiderDao
    {
        #region Constructor
        /// <summary>
        /// RecommendedRiderDao Constructor
        /// </summary>
        /// <param name="dataContext">HorseSpot Relational Database Context</param>
        public RecommendedRiderDao(HorseSpotDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion
    }
}
