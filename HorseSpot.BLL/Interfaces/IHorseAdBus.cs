using System.Threading.Tasks;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Interfaces
{
    public interface IHorseAdBus
    {
        Task Add(HorseAdDTO horseAdDTO, string userId);
        Task Update(int id, HorseAdDTO horseAdDTO, string userId);
        Task Delete(int id, string userId, bool isSold);
        Task Validate(int id);
        Task AddToFavorite(int id, string userId);
        HorseAdDTO GetById(int id);
        GetHorseAdListResultsDTO GetAllForAdmin(int pageNumber);
        GetHorseAdListResultsDTO SearchHorses(HorseAdSearchViewModel searchModel);
        bool CheckPostOwner(int adId, string userId);
        Task IncreaseViews(int id);
        Task SaveNewImage(int adId, string imageName, string userId);
        string DeleteImage(int imageId, string userId);
        void SetHorseAdProfilePicture(int imageId, string userId);
        LatestHorsesHomePageViewModel GetLatestHorsesForHomePage();
    }
}
