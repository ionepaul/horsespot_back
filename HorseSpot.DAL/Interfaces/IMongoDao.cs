using HorseSpot.DAL.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace HorseSpot.DAL.Interfaces
{
    public interface IMongoDao<T> where T : AdvertismentBaseClass
    {
        void Insert(T entity);
        void Delete(T entity);
        void Update(T entity);
        T GetById(string id);
        void Validate(ObjectId id);
        void UpdateFavoritesList(ObjectId id, IList<string> favoriteFor);
        void SetImages(string id, List<string> imagesIds);
        Task IncreaseViews(ObjectId id, int views);
    }
}
