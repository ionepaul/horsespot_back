using System.Collections.Generic;

namespace HorseSpot.Models.Models
{
    public class HorseAdSearchViewModel
    {
        public int PageNumber { get; set; }
        public string Gender { get; set; }
        public BetweenAge AgeModel { get; set; }
        public BetweenHeight HeightModel { get; set; }
        public BetweenPrice PriceModel { get; set; }
        public string Breed { get; set; }
        public int AbilityId { get; set; }
        public bool ToHaveXRays { get; set; }
        public bool ToHaveCompetionalExperience { get; set; }
        public List<int> SuitableFor { get; set; }
        public int PriceRangeId { get; set; }
        public bool ToHaveVideo { get; set; }
        public string AfterFatherName { get; set; }
        public string Country { get; set; }
        public SortModel SortModel { get; set; }

        public HorseAdSearchViewModel()
        {
            PageNumber = 1;
            AgeModel = new BetweenAge();
            HeightModel = new BetweenHeight();
            ToHaveXRays = false;
            ToHaveVideo = false;
            ToHaveCompetionalExperience = false;
            PriceRangeId = 0;
            AbilityId = 0;
            SortModel = new SortModel();
        }
    }

    public class BetweenAge
    {
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }

        public BetweenAge()
        {
            MinAge = 0;
            MaxAge = 0;
        }
    }

    public class BetweenHeight
    {
        public int? MinHeight { get; set; }
        public int? MaxHeight { get; set; }

        public BetweenHeight()
        {
            MinHeight = 0;
            MaxHeight = 0;
        }
    }

    public class BetweenPrice
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public BetweenPrice()
        {
            MinPrice = 0M;
            MaxPrice = 0M;
        }
    }

    public class SortModel
    {
        public int SortAfter { get; set; }
        public int SortDirection { get; set; }

        public SortModel()
        {
            SortAfter = 0;
            SortDirection = 1;
        }
    }
}
