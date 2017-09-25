using HorseSpot.DAL.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HorseSpot.DAL.Entities
{
    public class ImageModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageId { get; set; }
        public string Name { get; set; }
        public bool IsProfilePic { get; set; }
        public int HorseAdId { get; set; }

        [ForeignKey("HorseAdId")]
        public virtual HorseAd HorseAd { get; set; }
    }
}
