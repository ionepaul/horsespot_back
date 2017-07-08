﻿using System.Collections.Generic;

namespace HorseSpot.DAL.Models
{
    public class SearchModelDao
    {
        public int PageNumber { get; set; }
        public int GenderId { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public int? MinHeight { get; set; }
        public int? MaxHeight { get; set; }
        public string Breed { get; set; }
        public int AbilityId { get; set; }
        public bool ToHaveXRays { get; set; }
        public bool ToHaveCompetionalExperience { get; set; }
        public List<int> SuitableFor { get; set; }
        public int PriceRangeId { get; set; }
        public bool ToHaveVideo { get; set; }
        public string AfterFatherName { get; set; }
        public string Country { get; set; }
        public string SortAfter { get; set; }
        public int SortDirection { get; set; }
    }
}
