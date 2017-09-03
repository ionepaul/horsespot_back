using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace HorseSpot.Models.Models
{
    public class HorseAdDTO
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Title { get; set; }

        [Required]
        public string HorseName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public string Breed { get; set; }

        [Required]
        public int HeightInCm { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public IEnumerable<HorseAbilityDTO> Abilities { get; set; }

        public string VideoLink { get; set; }

        public PedigreeDTO Pedigree { get; set; }

        [Required]
        public bool HaveXRays { get; set; }

        [Required]
        public AddressDTO Address { get; set; }

        [Required]
        public bool HaveCompetionalExperience { get; set; }

        [Required]
        public IEnumerable<RecommendedRiderDTO> RecomendedRiders { get; set; }
        
        public IEnumerable<string> FavoritesFor { get; set; }

        public decimal Price { get; set; }

        [Required]
        public PriceRangeDTO PriceRange { get; set; }

        public IList<string> ImageIds { get; set; }

        public bool IsSponsorized { get; set; }

        [XmlIgnore]
        public int Views { get; set; }

        [XmlIgnore]
        public int CountFavoritesFor { get; set; }

        [XmlIgnore]
        public bool IsValidated { get; set; }

        [XmlIgnore]
        public DateTime DatePosted { get; set; }
    }
}
