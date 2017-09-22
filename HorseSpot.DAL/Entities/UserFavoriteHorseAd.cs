using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HorseSpot.DAL.Models;

namespace HorseSpot.DAL.Entities
{
    public class UserFavoriteHorseAd
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; }
        [Key, Column(Order = 1)]
        public int HorseAdId { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }

        [ForeignKey("HorseAdId")]
        public virtual HorseAd FavoriteHorseAd { get; set; }
    }
}

