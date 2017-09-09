using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;

namespace HorseSpot.DAL.Dao
{
    public class HorseAbilityDao : AbstractDao<HorseAbility>, IHorseAbilityDao
    {
        #region Constructor
        /// <summary>
        /// RecommendedRiderDao Constructor
        /// </summary>
        /// <param name="dataContext">HorseSpot Relational Database Context</param>
        public HorseAbilityDao(HorseSpotDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion
    }
}
