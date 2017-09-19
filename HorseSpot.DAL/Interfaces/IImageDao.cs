using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Models;

namespace HorseSpot.DAL.Interfaces
{
    public interface IImageDao : IDao<Image>
    {
        void Update(Image image); 
    }
}
