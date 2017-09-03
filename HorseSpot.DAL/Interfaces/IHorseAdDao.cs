using HorseSpot.DAL.Models;
namespace HorseSpot.DAL.Interfaces
{
    public interface IHorseAdDao : IDao<HorseAd>
    {
        GetHorseAdListResults GetFavoritesFor(string userId, int pageNumber);

        GetHorseAdListResults GetPostedAdsFor(string id, int pageNumber);

        GetHorseAdListResults GetAllForAdmin(int pageNumber);

        GetHorseAdListResults SearchAfter(SearchHorseDao searchQuery, int pageNumber);
    }
}
