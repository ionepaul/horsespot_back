using HorseSpot.DAL.Entities;
using System.Threading.Tasks;
using HorseSpot.DAL.Models;

namespace HorseSpot.DAL.Interfaces
{
    public interface IHorseAdDao : IDao<HorseAd>
    {
        GetHorseAdListResults GetFavoritesFor(string userId, int pageNumber);
        GetHorseAdListResults GetAllForAdmin(int pageNumber);
        GetHorseAdListResults SearchAfter(SearchHorseDao searchQuery, int pageNumber);
        Task UpdateAsync(HorseAd horseAd);
        void AddHorse(HorseAd horseAd);
    }
}
