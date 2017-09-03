using System.ComponentModel.DataAnnotations;

namespace HorseSpot.Models.Models
{
    public class PriceRangeDTO
    {
        [Required]
        public int Id { get; set; }

        public string PriceRangeValue { get; set; }
    }
}
