using HorseSpot.DAL.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HorseSpot.DAL.Entities
{
    public class PriceRange
    {
        [Key, ForeignKey("HorseAds")]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string PriceRangeValue { get; set; }

        public virtual ICollection<HorseAd> HorseAds { get; set; }
    }
}
