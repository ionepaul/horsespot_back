using HorseSpot.DAL.Interfaces;
using HorseSpot.DAL.Models;
using HorseSpot.Infrastructure.Constants;
using LinqKit;
using MongoDB.Driver;
using System.Linq;

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
            results.TotalCount = totalNumber;
            results.HorseAdList = _ctx.Users.Where(u=> u.Id ==userId).Select(x => x.FavoriteHorseAds).FirstOrDefault()
                                      .OrderByDescending(e => e.DatePosted).Skip(skipNumber).ToList().Take(ApplicationConstants.AdsPerPage);

            return results;
        }

        /// <summary>
        /// Gets the advertisments that were posted by the user by page number
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="pageNumber">Page Number</param>
        /// <returns>Model containing total number of ads posted and list of ads</returns>
        public GetHorseAdListResults GetPostedAdsFor(string id, int pageNumber)
        {
            var skipNumber = GetNumberToSkip(pageNumber);

            var totalNumber = _ctx.HorseAds.Where(e => e.UserId == id).ToList().Count;

            var results = new GetHorseAdListResults();
            results.TotalCount = totalNumber;
            results.HorseAdList = _ctx.HorseAds.Where(e => e.UserId == id).OrderByDescending(e=> e.Description).Skip(skipNumber).ToList().Take(ApplicationConstants.AdsPerPage);

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
            var orderPredicate = searchQuery.GetOrderPredicate();
            var isAscendingSortOrder = searchQuery.IsAscendingSortOrder();

            var totalNumber = _ctx.HorseAds.AsQueryable().AsExpandable().Where(searchPredicate).ToList().Count;

            results.TotalCount = totalNumber;

            if (isAscendingSortOrder)
            {
                results.HorseAdList = _ctx.HorseAds.AsQueryable().AsExpandable().Where(searchPredicate)
                                      .OrderBy(orderPredicate).Skip(skipNumber).Take(ApplicationConstants.AdsPerPage).ToList();
            }
            else
            {
                results.HorseAdList = _ctx.HorseAds.AsQueryable().AsExpandable().Where(searchPredicate)
                                      .OrderByDescending(orderPredicate).Skip(skipNumber).Take(ApplicationConstants.AdsPerPage).ToList();
            }

            return results;
        }

        #endregion

        #region PrivateMethods
        
        /// <summary>
        /// Gets the number of pages to skip
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <returns>Number of pagest to skip</returns>
        private int GetNumberToSkip(int pageNumber)
        {
            return (pageNumber - 1) * ApplicationConstants.AdsPerPage;
        }

        #endregion
    }
}
