using System.ComponentModel.DataAnnotations;

namespace HorseSpot.Models.Models
{
    public class GenderDTO
    {
        [Required]
        public int Id { get; set; }

        public string Gender { get; set; }
    }
}
