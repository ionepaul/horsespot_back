using System.ComponentModel.DataAnnotations;

namespace HorseSpot.DAL.Entities
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string CountryName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Capital { get; set; }

        [Required]
        [MaxLength(100)]
        public string TimezoneId { get; set; }
    }
}
