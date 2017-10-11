using System.Collections.Generic;
using System.Threading.Tasks;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Models;
using HorseSpot.DAL.Search;

namespace HorseSpot.DAL.Interfaces
{
    public interface IHorseAdDao : IDao<HorseAd>
    {
        GetHorseAdListResults GetAllForAdmin(int pageNumber);
        GetHorseAdListResults SearchAfter(SearchHorseDao searchQuery, int pageNumber);
        Task UpdateAsync(HorseAd horseAd);
        Task AddHorse(HorseAd horseAd);
        Dictionary<string, IEnumerable<HorseAd>> GetLatestHorses();
    }
}
