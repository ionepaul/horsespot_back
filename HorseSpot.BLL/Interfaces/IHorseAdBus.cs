using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HorseSpot.BLL.Interfaces
{
    public interface IHorseAdBus
    {
        Task<string> Add(HorseAdDTO horseAdDTO, string userId);
        Task Update(int id, HorseAdDTO horseAdDTO, string userId);
        Task Delete(int id, string userId, bool isSold);
        Task Validate(int id);
        Task AddToFavorite(int id, string userId);
        HorseAdDTO GetById(int id);
        GetHorseAdListResultsDTO GetUserFavorites(int pageNumber, string userId);
        GetHorseAdListResultsDTO GetAllForUser(int pageNumber, string userId);
        GetHorseAdListResultsDTO GetAllForAdmin(int pageNumber);
        GetHorseAdListResultsDTO SearchHorses(HorseAdSearchViewModel searchModel);
        bool CheckPostOwner(int adId, string userId);
        Task IncreaseViews(int id);
    }
}
