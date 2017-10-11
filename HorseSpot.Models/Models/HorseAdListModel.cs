using System;

namespace HorseSpot.Models.Models
{
    public class HorseAdListModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string HorseName { get; set; }

        public int Age { get; set; }

        public string Breed { get; set; }

        public string Price { get; set; }

        public string PriceRange { get; set; }

        public string Country { get; set; }

        public string ImageId { get; set; }

        public bool IsValidated { get; set; }

        public string UserId { get; set; }

        public int CountFavoritesFor { get; set; }

        public int Views { get; set; }

        public string Gender { get; set; }

        public DateTime DatePosted { get; set; }

        public string Description { get; set; }
    }
}
