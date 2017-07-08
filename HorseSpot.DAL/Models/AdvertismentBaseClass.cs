using HorseSpot.DAL.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace HorseSpot.DAL.Models
{
    public class AdvertismentBaseClass
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
    }
}
