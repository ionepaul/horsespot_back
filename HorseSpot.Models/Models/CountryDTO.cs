using System.ComponentModel.DataAnnotations;

namespace HorseSpot.Models.Models
{
    public class CountryDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string CountryName { get; set; }
    }
}
