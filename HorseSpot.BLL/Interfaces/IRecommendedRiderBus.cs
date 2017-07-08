using HorseSpot.Models.Models;
using System.Collections.Generic;

namespace HorseSpot.BLL.Interfaces
{
    public interface IRecommendedRiderBus
    {
        IEnumerable<RecommendedRiderDTO> GetAll();
    }
}
