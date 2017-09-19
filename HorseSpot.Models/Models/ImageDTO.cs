using System.ComponentModel.DataAnnotations;

namespace HorseSpot.Models.Models
{
    public class ImageDTO
    {
        public int ImageId { get; set; }
        [Required]
        public string ImageName { get; set; }
        public bool IsProfilePic { get; set; }
    }
}
