using System.ComponentModel.DataAnnotations;

namespace HorseSpot.DAL.Entities
{
    public class PriceRange
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string PriceRangeValue { get; set; }
    }
}
