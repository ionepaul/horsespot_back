using System.ComponentModel.DataAnnotations;

namespace HorseSpot.DAL.Entities
{
    public class Gender
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string GenderValue { get; set; }
    }
}
