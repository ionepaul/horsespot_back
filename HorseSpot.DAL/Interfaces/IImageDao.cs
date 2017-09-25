using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Models;

namespace HorseSpot.DAL.Interfaces
{
    public interface IImageDao : IDao<ImageModel>
    {
        void Update(ImageModel image); 
    }
}
