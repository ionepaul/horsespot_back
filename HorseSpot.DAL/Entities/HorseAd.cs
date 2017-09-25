using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HorseSpot.DAL.Entities;

namespace HorseSpot.DAL.Models
{
    public class HorseAd
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime DatePosted { get; set; }
        public decimal Price { get; set; }
        public string VideoLink { get; set; }
        public int Views { get; set; }
        public bool IsSponsorized { get; set; }
        [Required]
        public bool IsValidated { get; set; }
        [Required]
        public string HorseName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public int Height { get; set; }
        [Required]
        public string Breed { get; set; }
        public bool HaveXRays { get; set; }
        public bool HaveCompetionalExperience { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSold { get; set; }
        public int AddressId { get; set; }
        public int PedigreeId { get; set; }
        public int PriceRangeId { get; set; }

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
        [ForeignKey("PedigreeId")]
        public virtual Pedigree Pedigree { get; set; }
        [ForeignKey("PriceRangeId")]
        public virtual PriceRange PriceRange { get; set; }
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }

        public virtual ICollection<ImageModel> Images { get; set; }
        public virtual ICollection<HorseAbility> Abilities { get; set; }
        public virtual ICollection<RecommendedRider> RecomendedRiders { get; set; }
        public virtual ICollection<UserFavoriteHorseAd> FavoriteFor { get; set; }

        [NotMapped]
        public IEnumerable<int> RecommendedRiderIds { get; set; }
        [NotMapped]
        public IEnumerable<int> HorseAbilitesIds { get; set; }
    }
}
