using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HorseSpot.DAL.Models
{
    public class SearchHorseDao : ISearchHelper<HorseAd>
    {
        #region Local Variables

        private string _gender;
        private int _minAge;
        private int _maxAge;
        private int _minHeight;
        private int _maxHeight;
        private string _breed;
        private int _horseAbilityId;
        private bool _haveXRays;
        private bool _haveCompetitionalExperience;
        private List<int> _recommendedRider;
        private int _priceRangeId;
        private bool _haveVideo;
        private string _afterFather;
        private string _country;
        private string _sortAfter;
        private int _sortDirection;

        #endregion

        #region Constructor

        public SearchHorseDao(SearchModelDao searchModel)
        {
            _gender = searchModel.Gender;
            _minAge = searchModel.MinAge == null ? 0 : searchModel.MinAge.Value;
            _maxAge = searchModel.MaxAge == null ? 0 : searchModel.MaxAge.Value;
            _minHeight = searchModel.MinHeight == null ? 0 : searchModel.MinHeight.Value;
            _maxHeight = searchModel.MaxHeight == null ? 0 : searchModel.MaxHeight.Value;
            _breed = searchModel.Breed;
            _horseAbilityId = searchModel.AbilityId;
            _haveXRays = searchModel.ToHaveXRays;
            _haveCompetitionalExperience = searchModel.ToHaveCompetionalExperience;
            _recommendedRider = searchModel.SuitableFor;
            _priceRangeId = searchModel.PriceRangeId;
            _haveVideo = searchModel.ToHaveVideo;
            _afterFather = searchModel.AfterFatherName;
            _country = searchModel.Country;
            _sortAfter = searchModel.SortAfter;
            _sortDirection = searchModel.SortDirection;
        }

        #endregion

        private Expression<Func<HorseAd, bool>> CheckValidation()
        {

            var subpredicate = PredicateBuilder.True<HorseAd>();

            subpredicate = subpredicate.And(ad => ad.IsValidated);

            return subpredicate.Expand();
        }

        private Expression<Func<HorseAd, bool>> GenderSearch()
        {
            var genderSearch = PredicateBuilder.True<HorseAd>();

            if (!string.IsNullOrEmpty(_gender))
            {
                genderSearch = genderSearch.And(ad => ad.Gender.Equals(_gender));
            }

            return genderSearch.Expand();
        }

        private Expression<Func<HorseAd, bool>> AgeSearch()
        {
            var ageSearch = PredicateBuilder.True<HorseAd>();

            if (_minAge != 0 && _maxAge != 0)
            {
                ageSearch = ageSearch.And(ad => ad.Age >= _minAge && ad.Age <= _maxAge);

            }
            else if (_minAge == 0 && _maxAge != 0)
            {
                ageSearch = ageSearch.And(ad => ad.Age <= _maxAge);
            }
            else if (_minAge != 0 && _maxAge == 0)
            {
                ageSearch = ageSearch.And(ad => ad.Age >= _minAge);
            }

            return ageSearch.Expand();
        }

        private Expression<Func<HorseAd, bool>> HeightSearch()
        {
            var heightSearch = PredicateBuilder.True<HorseAd>();

            if (_minHeight != 0 && _maxHeight != 0)
            {
                heightSearch = heightSearch.And(ad => ad.Height >= _minHeight && ad.Height <= _maxHeight);
            }
            else if (_minHeight == 0 && _maxHeight != 0)
            {
                heightSearch = heightSearch.And(ad => ad.Height <= _maxHeight);
            }
            else if (_minHeight != 0 && _maxHeight == 0)
            {
                heightSearch = heightSearch.And(ad => ad.Height >= _minHeight);
            }

            return heightSearch.Expand();
        }

        private Expression<Func<HorseAd, bool>> BreedSearch()
        {
            var breedSearch = PredicateBuilder.True<HorseAd>();

            if (!string.IsNullOrEmpty(_breed) && !string.IsNullOrWhiteSpace(_breed))
            {
                breedSearch = breedSearch.And(ad => ad.Breed == _breed);
            }

            return breedSearch.Expand();
        }

        private Expression<Func<HorseAd, bool>> CountrySearch()
        {
            var countrySearch = PredicateBuilder.True<HorseAd>();

            if (!string.IsNullOrEmpty(_country) && !string.IsNullOrWhiteSpace(_country))
            {
                countrySearch = countrySearch.And(ad => ad.Address.Country == _country);
            }

            return countrySearch.Expand();
        }

        private Expression<Func<HorseAd, bool>> XRaysSearch()
        {
            var xRaysSearch = PredicateBuilder.True<HorseAd>();

            if (_haveXRays)
            {
                xRaysSearch = xRaysSearch.And(ad => ad.HaveXRays);
            }

            return xRaysSearch.Expand();
        }

        private Expression<Func<HorseAd, bool>> CompetitionalExperienceSearch()
        {
            var experienceSearch = PredicateBuilder.True<HorseAd>();

            if (_haveCompetitionalExperience)
            {
                experienceSearch = experienceSearch.And(ad => ad.HaveCompetionalExperience);
            }

            return experienceSearch.Expand();
        }

        private Expression<Func<HorseAd, bool>> RecommendedRiderSearch()
        {
            var riderSearch = PredicateBuilder.False<HorseAd>();

            foreach (var rider in _recommendedRider)
            {
                Expression<Func<RecommendedRider, bool>> riderHelper = x => x.Id == rider;
                Expression<Func<HorseAd, bool>> riderSearchHelper =
                    ad => ad.RecomendedRiders.Any(recommendedRider => riderHelper.Invoke(recommendedRider));

                riderSearch = riderSearch.Or(riderSearchHelper.Expand());
            }

            return riderSearch.Expand();
        }

        private Expression<Func<HorseAd, bool>> PriceRangeSearch()
        {
            var priceRangeSearch = PredicateBuilder.True<HorseAd>();

            if (_priceRangeId != 0)
            {
                priceRangeSearch = priceRangeSearch.And(ad => ad.PriceRange.Id == _priceRangeId);
            }

            return priceRangeSearch.Expand();
        }

        private Expression<Func<HorseAd, bool>> VideoSearch()
        {
            var videoSearch = PredicateBuilder.True<HorseAd>();

            if (_haveVideo)
            {
                videoSearch = videoSearch.And(ad => !string.IsNullOrEmpty(ad.VideoLink) && !string.IsNullOrWhiteSpace(ad.VideoLink));
            }

            return videoSearch.Expand();
        }

        private Expression<Func<HorseAd, bool>> FatherNameSearch()
        {
            var fatherSearch = PredicateBuilder.True<HorseAd>();

            if (!string.IsNullOrEmpty(_afterFather) && !string.IsNullOrWhiteSpace(_afterFather))
            {
                fatherSearch = fatherSearch.And(ad => ad.Pedigree != null && ad.Pedigree.Father != null
                              && ad.Pedigree.Father != String.Empty && ad.Pedigree.Father.ToLower() == _afterFather.ToLower());
            }

            return fatherSearch.Expand();
        }

        public Expression<Func<HorseAd, bool>> GetSearchPredicate()
        {
            var predicate = PredicateBuilder.True<HorseAd>();
            //Validation
            predicate = predicate.And(CheckValidation());
            //Gender
            predicate = predicate.And(GenderSearch());
            //Age
            predicate = predicate.And(AgeSearch());
            //Height
            predicate = predicate.And(HeightSearch());
            //Breed
            predicate = predicate.And(BreedSearch());
            //Country
            predicate = predicate.And(CountrySearch());
            //XRays
            predicate = predicate.And(XRaysSearch());
            //Competitional Experience
            predicate = predicate.And(CompetitionalExperienceSearch());
            //Price Range
            predicate = predicate.And(PriceRangeSearch());
            //Video
            predicate = predicate.And(VideoSearch());
            //FatherName 
            predicate = predicate.And(FatherNameSearch());

            //Recommended Rider
            if (_recommendedRider != null && _recommendedRider.Count != 0)
            {
                predicate = predicate.And(RecommendedRiderSearch());
            }

            //Ability
            if (_horseAbilityId != 0)
            {
                Expression<Func<HorseAbility, bool>> abilityHelper = x => x.Id == _horseAbilityId;
                Expression<Func<HorseAd, bool>> abilitySearch =
                    ad => ad.Abilities.Any(ability => abilityHelper.Invoke(ability));

                predicate = predicate.And(abilitySearch.Expand());
            }

            return predicate;
        }

        /// <summary>
        /// Creates the order predicate
        /// </summary>
        /// <returns>Order expresion to be applied in sort</returns>
        public Expression<Func<HorseAd, object>> GetOrderPredicate()
        {
            var param = Expression.Parameter(typeof(HorseAd), "horseAd");

            var orderExpression = Expression.Lambda<Func<HorseAd, object>>(Expression.Convert(Expression.Property(param, _sortAfter), typeof(object)), param);

            return orderExpression;
        }

        /// <summary>
        /// Check if ascending order
        /// </summary>
        /// <returns>True/False</returns>
        public bool IsAscendingSortOrder()
        {
            if (_sortDirection == 0)
            {
                return true;
            }

            return false;
        }
    }
}
