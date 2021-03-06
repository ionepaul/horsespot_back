﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HorseSpot.DAL.Entities
{
    public class HorseAbility
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Ability { get; set; }

        public virtual ICollection<HorseAd> HorseAds { get; set; }
    }
}
