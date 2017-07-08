using HorseSpot.DAL.Entities;
using System.Collections.Generic;

namespace HorseSpot.DAL.Models
{
    public class HorseAd : AdvertismentBaseClass
    {
        public string HorseName { get; set; }

        public Gender Gender { get; set; }

        public int Age { get; set; }

        public int Height { get; set; }

        public string Breed { get; set; }

        public IEnumerable<HorseAbility> Abilities { get; set; }

        public Pedigree Pedigree { get; set; }

        public bool HaveXRays { get; set; }

        public bool HaveCompetionalExperience { get; set; }

        public IEnumerable<RecommendedRider> RecomendedRiders { get; set; }        
    }
}
