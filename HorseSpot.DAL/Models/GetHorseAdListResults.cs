using System.Collections.Generic;

namespace HorseSpot.DAL.Models
{
    public class GetHorseAdListResults
    {
        public IEnumerable<HorseAd> HorseAdList { get; set; }

        public int TotalCount { get; set; }
    }
}
