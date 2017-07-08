using System.Collections.Generic;

namespace HorseSpot.BLL.Interfaces
{
    public interface ICountryBus
    {
        IEnumerable<string> GetAll();
    }
}
