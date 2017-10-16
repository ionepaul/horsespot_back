using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.DAL.Models;
using HorseSpot.DAL.Search;
using HorseSpot.Infrastructure.Constants;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.Resources;
using LinqKit;

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

        public async Task EditHorseAdAsync(HorseAd horseAd)
        {
            var updatedHorseAd = CheckAndAddAbilitiesAndRecommendedRiders(horseAd, true);

            _ctx.Entry(updatedHorseAd).State = EntityState.Modified;

            await _ctx.SaveChangesAsync();
        }

        public async Task AddHorse(HorseAd horseAd)
        {
            var horseAdToSave = CheckAndAddAbilitiesAndRecommendedRiders(horseAd, false);

            _ctx.HorseAds.Add(horseAdToSave);

            await _ctx.SaveChangesAsync();
        }

        public Dictionary<string, IEnumerable<HorseAd>> GetLatestHorses()
        {
            var horses = _ctx.HorseAds.OrderByDescending(x => x.DatePosted).Where(x => x.IsValidated);

            var showJumping = horses.Where(x => x.Abilities.Any(y => y.Id == 1)).Take(2).AsEnumerable();

            var dressage = horses.Where(x => x.Abilities.Any(y => y.Id == 2) && !showJumping.Contains(x)).Take(2).AsEnumerable();

            var eventing = horses.Where(x => x.Abilities.Any(y => y.Id == 3) && !dressage.Contains(x) && !showJumping.Contains(x)).Take(2).AsEnumerable();

            var endurance = horses.Where(x => x.Abilities.Any(y => y.Id == 4) && !dressage.Contains(x) && !showJumping.Contains(x) && !eventing.Contains(x)).Take(2).AsEnumerable();

            var latestHorsesDictionary = new Dictionary<string, IEnumerable<HorseAd>>();

            latestHorsesDictionary.Add(ApplicationConstants.LatestDictionaryShowJumpingKey, showJumping);
            latestHorsesDictionary.Add(ApplicationConstants.LatestDictionaryDressageKey, dressage);
            latestHorsesDictionary.Add(ApplicationConstants.LatestDictionaryEventingKey, eventing);
            latestHorsesDictionary.Add(ApplicationConstants.LatestDictionaryEnduranceKey, endurance);

            return latestHorsesDictionary;
        }

        #endregion

        #region Private Methods

        private int GetNumberToSkip(int pageNumber)
        {
            return (pageNumber - 1) * ApplicationConstants.AdsPerPage;
        }

        private HorseAd CheckAndAddAbilitiesAndRecommendedRiders(HorseAd horseAd, bool isUpdate)
        {
            var priceRange = _ctx.PriceRanges.Find(horseAd.PriceRangeId);

            if (priceRange == null)
            {
                throw new ValidationException(Resources.InvalidPriceRangeIdentifier);
            }

            if (isUpdate)
            {
                horseAd.RecomendedRiders.Clear();
                horseAd.Abilities.Clear();
            }
            else
            {
                horseAd.RecomendedRiders = new List<RecommendedRider>();
                horseAd.Abilities = new List<HorseAbility>();
            }

            horseAd.HorseAbilitesIds.ForEach(id =>
            {
                var horseAbility = _ctx.HorseAbilities.Find(id);

                if (horseAbility == null)
                {
                    throw new ValidationException(Resources.InvalidAbilityIdentifier);
                }

                horseAd.Abilities.Add(horseAbility);
            });

            horseAd.RecommendedRiderIds.ForEach(id =>
            {
                var recommendedRider = _ctx.RecommendedRiders.Find(id);

                if (recommendedRider == null)
                {
                    throw new ValidationException(Resources.InvalidRecommendedRiderIdentifier);
                }

                horseAd.RecomendedRiders.Add(recommendedRider);
            });

            return horseAd;
        }

        #endregion
    }
}
