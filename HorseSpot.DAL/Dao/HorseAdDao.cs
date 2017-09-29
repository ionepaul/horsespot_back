using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.DAL.Models;
using HorseSpot.Infrastructure.Constants;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.Resources;

namespace HorseSpot.DAL.Dao
{
    public class HorseAdDao : AbstractDao<HorseAd>, IHorseAdDao
    {
        #region Constructor

        public HorseAdDao(HorseSpotDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion

        #region Public Methods

        public GetHorseAdListResults GetAllForAdmin(int pageNumber)
        {
            var skipNumber = GetNumberToSkip(pageNumber);

            var horseAds = _ctx.HorseAds.Where(e => !e.IsValidated);

            var results = new GetHorseAdListResults()
            {
                TotalCount = horseAds.Count(),
                HorseAdList = horseAds.OrderByDescending(e => e.DatePosted).Skip(skipNumber).ToList().Take(ApplicationConstants.AdsPerPage)
            };   

            return results;
        }

        public GetHorseAdListResults SearchAfter(SearchHorseDao searchQuery, int pageNumber)
        {
            var skipNumber = GetNumberToSkip(pageNumber);

            var searchPredicate = searchQuery.GetSearchPredicate();
            var orderProperty = searchQuery.GetOrderProperty();
            var isAscendingSortOrder = searchQuery.IsAscendingSortOrder();

            var horseAds = _ctx.HorseAds.AsQueryable().AsExpandable().Where(searchPredicate);

            var results = new GetHorseAdListResults()
            {
                TotalCount = horseAds.Count(),
                HorseAdList = isAscendingSortOrder ? horseAds.AsEnumerable().OrderBy(x => orderProperty.GetValue(x, null)).Skip(skipNumber).Take(ApplicationConstants.AdsPerPage).ToList()
                                                   : horseAds.AsEnumerable().OrderByDescending(x => orderProperty.GetValue(x, null)).Skip(skipNumber).Take(ApplicationConstants.AdsPerPage).ToList()
            };

            return results;
        }

        public async Task UpdateAsync(HorseAd horseAd)
        {
            _ctx.Entry(horseAd).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
        }

        public void AddHorse(HorseAd horseAd)
        {
            horseAd.RecomendedRiders = new List<RecommendedRider>();
            horseAd.Abilities = new List<HorseAbility>();

            var priceRange = _ctx.PriceRanges.FirstOrDefault(p => p.Id == horseAd.PriceRangeId);
            
            if (priceRange == null)
            {
                throw new ValidationException(Resources.InvalidPriceRangeIdentifier);
            }

            horseAd.HorseAbilitesIds.ForEach(id =>
            {
                var horseAbility = _ctx.HorseAbilities.FirstOrDefault(a => a.Id == id);

                if (horseAbility == null)
                {
                    throw new ValidationException(Resources.InvalidAbilityIdentifier);
                }

                horseAd.Abilities.Add(horseAbility);
            });

            horseAd.RecommendedRiderIds.ForEach(id =>
            {
                var recommendedRider = _ctx.RecommendedRiders.FirstOrDefault(r => r.Id == id);

                if (recommendedRider == null)
                {
                    throw new ValidationException(Resources.InvalidRecommendedRiderIdentifier);
                }

                horseAd.RecomendedRiders.Add(recommendedRider);
            });

            _ctx.HorseAds.Add(horseAd);
            _ctx.SaveChanges();
        }

        #endregion

        #region Private Methods

        private int GetNumberToSkip(int pageNumber)
        {
            return (pageNumber - 1) * ApplicationConstants.AdsPerPage;
        }

        #endregion
    }
}
