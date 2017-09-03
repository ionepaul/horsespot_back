using HorseSpot.Models.Models;
using System.Collections.Generic;

namespace HorseSpot.BLL.Interfaces
{
    public interface IHorseAbilityBus
    {
        IEnumerable<HorseAbilityDTO> GetAll();
    }
}
