using System.Collections.Generic;
using System.Linq;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;

namespace HorseSpot.DAL.Dao
{
    public class UtilDao : IUtilDao
    {
        private readonly HorseSpotDataContext _ctx;

        public UtilDao(HorseSpotDataContext dataContext)
        {
            _ctx = dataContext;
        }

        public IEnumerable<HorseAbility> GetAllAbilities()
        {
            return _ctx.HorseAbilities.AsEnumerable();
        }

        public IEnumerable<Country> GetAllCountries()
        {
            return _ctx.Countries.AsEnumerable();
        }

        public IEnumerable<PriceRange> GetAllPriceRanges()
        {
            return _ctx.PriceRanges.AsEnumerable();
        }

        public IEnumerable<RecommendedRider> GetAllRecommendedRiders()
        {
            return _ctx.RecommendedRiders.AsEnumerable();
        }
    }
}
