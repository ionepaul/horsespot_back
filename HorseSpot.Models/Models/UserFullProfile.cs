﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseSpot.Models.Models
{
    public class UserFullProfile
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalForSale { get; set; }
        public int TotalReferenes { get; set; }
        public int TotalMeetings { get; set; }
        public int TotalFeedback { get; set; }
        public IEnumerable<HorseAdListModel> HorsesForSale { get; set; }
        public IEnumerable<HorseAdListModel> FavoriteHorses { get; set; }
        public IEnumerable<HorseAdListModel> ReferenceHorses { get; set; }
    }
}
