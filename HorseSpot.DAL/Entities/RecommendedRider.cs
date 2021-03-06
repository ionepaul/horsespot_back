﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HorseSpot.DAL.Entities
{
    public class RecommendedRider
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Rider { get; set; }

        public virtual ICollection<HorseAd> HorseAds { get; set; }
    }
}
