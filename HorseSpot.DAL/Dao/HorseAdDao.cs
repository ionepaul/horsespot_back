using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.DAL.Models;
using HorseSpot.Infrastructure.Constants;
using HorseSpot.Infrastructure.Resources;
using LinqKit;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using HorseSpot.Infrastructure.Exceptions;

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

        /// <summary>
        /// Gets the advertisments saved in the user's wish list by page number
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="pageNumber">Page Number</param>
        /// <returns>Model containing total number of ads in wish list and list of ads</returns>
        public GetHorseAdListResults GetFavoritesFor(string userId, int pageNumber)
        {
            var skipNumber = GetNumberToSkip(pageNumber);

            var totalNumber = _ctx.Users.Where(e=> e.Id == userId).Select(u => u.FavoriteHorseAds).FirstOrDefault().ToList().Count;

            var results = new GetHorseAdListResults();
            //results.TotalCount = totalNumber;
            //results.HorseAdList = _ctx.Users.Where(u=> u.Id ==userId).Select(x => x.FavoriteHorseAds).FirstOrDefault()
            //                          .OrderByDescending(e => e.FavoriteHorseAd.DatePosted).Skip(skipNumber).ToList().Take(ApplicationConstants.AdsPerPage);

            return results;
        }

        /// <summary>
        /// Gets all unvalidated advertisments by page number
        /// </summary>
        /// <param name="pageNumber">Page Number</param>
        /// <returns>Model Containing total number of unvalidated ads and list of ads</returns>
        public GetHorseAdListResults GetAllForAdmin(int pageNumber)
        {
            var skipNumber = GetNumberToSkip(pageNumber);

            var totalNumber = _ctx.HorseAds.Where(e => !e.IsValidated).ToList().Count;

            var results = new GetHorseAdListResults();
            results.TotalCount = totalNumber;
            results.HorseAdList = _ctx.HorseAds.Where(e => !e.IsValidated).OrderByDescending(e => e.DatePosted).Skip(skipNumber).ToList().Take(ApplicationConstants.AdsPerPage);

            return results;
        }

        /// <summary>
        /// Search horse by search criteria 
        /// </summary>
        /// <param name="searchQuery">Search query model</param>
        /// <param name="pageNumber">Page Number</param>
        /// <returns>Model containing total number of ads found and list of ads that match the serach criteria</returns>
        public GetHorseAdListResults SearchAfter(SearchHorseDao searchQuery, int pageNumber)
        {
            var skipNumber = GetNumberToSkip(pageNumber);

            var results = new GetHorseAdListResults();

            var searchPredicate = searchQuery.GetSearchPredicate();
            var orderProperty = searchQuery.GetOrderProperty();
            var isAscendingSortOrder = searchQuery.IsAscendingSortOrder();

            var totalNumber = _ctx.HorseAds.AsQueryable().AsExpandable().Where(searchPredicate).ToList().Count;

            results.TotalCount = totalNumber;

            if (isAscendingSortOrder)
            {
                results.HorseAdList = _ctx.HorseAds.AsQueryable().AsExpandable().Where(searchPredicate).AsEnumerable()
                                      .OrderBy(x => orderProperty.GetValue(x, null)).Skip(skipNumber).Take(ApplicationConstants.AdsPerPage).ToList();
            }
            else
            {
                results.HorseAdList = _ctx.HorseAds.AsQueryable().AsExpandable().Where(searchPredicate).AsEnumerable()
                                      .OrderByDescending(x => orderProperty.GetValue(x, null)).Skip(skipNumber).Take(ApplicationConstants.AdsPerPage).ToList();
            }

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
