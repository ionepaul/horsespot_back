using System.Collections.Generic;

namespace HorseSpot.Models.Models
{
    public class GetHorseAdListResultsDTO
    {
        public IEnumerable<HorseAdListModel> HorseAdList { get; set; }

        public int TotalCount { get; set; }
    }
}
