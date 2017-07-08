using System.ComponentModel.DataAnnotations;

namespace HorseSpot.Models.Models
{
    public class AddressDTO
    {
        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Street { get; set; }
    }
}
