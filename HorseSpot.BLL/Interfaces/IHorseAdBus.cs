using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HorseSpot.BLL.Interfaces
{
    public interface IHorseAdBus
    {
        Task<string> Add(HorseAdDTO horseAdDTO, string userId);
        void Update(string id, HorseAdDTO horseAdDTO, string userId);
        Task Delete(string id, string userId);
        Task Validate(string id);
        void AddToFavorite(string id, string userId);
        HorseAdDTO GetById(string id);
        GetHorseAdListResultsDTO GetUserFavorites(int pageNumber, string userId);
        GetHorseAdListResultsDTO GetAllForUser(int pageNumber, string userId);
        GetHorseAdListResultsDTO GetAllForAdmin(int pageNumber);
        GetHorseAdListResultsDTO SearchHorses(HorseAdSearchViewModel searchModel);
        bool CheckPostOwner(string adId, string userId);
        Task IncreaseViews(string id);
    }
}
