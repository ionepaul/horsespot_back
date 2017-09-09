using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HorseSpot.DAL.Entities
{
    public class PriceRange
    {
        [Key, ForeignKey("PriceRangeId")]
        public int PriceRangeId { get; set; }
        [Required]
        [MaxLength(100)]
        public string PriceRangeValue { get; set; }
    }
}
