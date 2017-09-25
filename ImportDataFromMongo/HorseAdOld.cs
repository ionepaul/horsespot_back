using System;
using System.Collections.Generic;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ImportDataFromMongo
{
    public class HorseAdOld
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string Title { get; set; }

        public string UserId { get; set; }

        public string Description { get; set; }

        public Address Address { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DatePosted { get; set; }

        [BsonRepresentation(BsonType.Int64)]
        public decimal Price { get; set; }

        public PriceRange PriceRange { get; set; }

        public string VideoLink { get; set; }

        public IList<string> ImageIds { get; set; }

        public IList<string> FavoriteFor { get; set; }

        public int Views { get; set; }

        public bool IsSponsorized { get; set; }

        public bool IsValidated { get; set; }

        public string HorseName { get; set; }

        public Gender Gender { get; set; }

        public int Age { get; set; }

        public int Height { get; set; }

        public string Breed { get; set; }

        public IEnumerable<HorseAbility> Abilities { get; set; }

        public PedigreeOld Pedigree { get; set; }

        public bool HaveXRays { get; set; }

        public bool HaveCompetionalExperience { get; set; }

        public IEnumerable<RecommendedRider> RecomendedRiders { get; set; }
    }
}
