using HorseSpot.DAL.Entities;

namespace HorseSpot.DAL.Interfaces
{
    public interface IImageDao : IDao<ImageModel>
    {
        void Update(ImageModel image); 
    }
}
