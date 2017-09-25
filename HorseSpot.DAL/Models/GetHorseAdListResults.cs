using System.Collections.Generic;
using HorseSpot.DAL.Entities;

namespace HorseSpot.DAL.Models
{
    public class GetHorseAdListResults
    {
        public IEnumerable<HorseAd> HorseAdList { get; set; }
        public int TotalCount { get; set; }
    }
}
