using System.Collections.Generic;
using HorseSpot.DAL.Entities;

namespace HorseSpot.DAL.Interfaces
{
    public interface IUtilDao
    {
        IEnumerable<HorseAbility> GetAllAbilities();
        IEnumerable<RecommendedRider> GetAllRecommendedRiders();
        IEnumerable<PriceRange> GetAllPriceRanges();
        IEnumerable<Country> GetAllCountries();
    }
}
