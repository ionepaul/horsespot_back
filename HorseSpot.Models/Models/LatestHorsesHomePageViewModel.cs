using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseSpot.Models.Models
{
    public class LatestHorsesHomePageViewModel
    {
        public IEnumerable<HorseAdListModel> LatestInShowJumping { get; set; }
        public IEnumerable<HorseAdListModel> LatestInDressage { get; set; }
        public IEnumerable<HorseAdListModel> LatestInEventing { get; set; }
        public IEnumerable<HorseAdListModel> LatestInEndurance { get; set; }
    }
}
