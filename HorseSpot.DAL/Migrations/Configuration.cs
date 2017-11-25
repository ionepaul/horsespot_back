namespace HorseSpot.DAL.Migrations
{
    using Entities;
    using Infrastructure.Constants;
    using Infrastructure.Utils;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<HorseSpotDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(HorseSpotDataContext context)
        {
            if (context.Clients.Count() > 0)
            {
                return;
            }
            else
            {
                context.Clients.AddRange(BuildClientsList());
            }

            if (context.HorseAbilities.Count() > 0)
            {
                return;
            }
            else
            {
                context.HorseAbilities.AddRange(BuildHorseAbilitiesList());
            }

            if (context.RecommendedRiders.Count() > 0)
            {
                return;
            }
            else
            {
                context.RecommendedRiders.AddRange(BuildRecommendedRidersList());
            }

            if (context.PriceRanges.Count() > 0)
            {
                return;
            }
            else
            {
                context.PriceRanges.AddRange(BuildPriceRangesList());
            }

            if (context.Countries.Count() > 0)
            {
                return;
            }
            else
            {
                context.Countries.AddRange(SetCountries());
            }

            context.SaveChanges();
        }

        private List<Country> SetCountries()
        {
            var countries = BuildCountriesList();
            var timezones = TimeZoneInfo.GetSystemTimeZones();

            foreach (var country in countries)
            {
                country.TimezoneId = "Empty";
                foreach (var timezone in timezones)
                {
                    if (timezone.DisplayName.IndexOf(country.Capital) > -1)
                    {
                        country.TimezoneId = timezone.Id;
                        break;
                    }
                }
            }

            return countries;
        }

        private static List<Country> BuildCountriesList()
        {
            #region List Creation
            List<Country> CountryList = new List<Country>
            {
                new Country
                {
                    CountryName = "Albania",
                    Capital = "Tirana"
                },
                new Country
                {
                    CountryName = "Andorra",
                    Capital = "Andorra la Vella"
                },
                new Country
                {
                    CountryName = "Armenia",
                    Capital = "Yerevan"
                },
                new Country
                {
                    CountryName = "Austria",
                    Capital = "Vienna"
                },
                new Country
                {
                    CountryName = "Azerbaijan",
                    Capital = "Baku"
                },
                new Country
                {
                    CountryName = "Belarus",
                    Capital = "Minsk"
                },
                new Country
                {
                    CountryName = "Belgium",
                    Capital = "Brussels"
                },
                new Country
                {
                    CountryName = "Bosnia & Herzegovina",
                    Capital = "Sarajevo"
                },
                new Country
                {
                    CountryName = "Bulgaria",
                    Capital = "Sofia"
                },
                new Country
                {
                    CountryName = "Croatia",
                    Capital = "Zagreb"
                },
                new Country
                {
                    CountryName = "Cyprus",
                    Capital = "Nicosia"
                },
                new Country
                {
                    CountryName = "Czech Republic",
                    Capital = "Prague"
                },
                new Country
                {
                    CountryName = "Denmark",
                    Capital = "Copenhagen"
                },
                new Country
                {
                    CountryName = "Estonia",
                    Capital = "Tallinn"
                },
                new Country
                {
                    CountryName = "Finland",
                    Capital = "Helsinki"
                },
                new Country
                {
                    CountryName = "France",
                    Capital = "Paris"
                },
                new Country
                {
                    CountryName = "Georgia",
                    Capital = "Tbilisi"
                },
                new Country
                {
                    CountryName = "Germany",
                    Capital = "Berlin"
                },
                new Country
                {
                    CountryName = "Greece",
                    Capital = "Athens"
                },
                new Country
                {
                    CountryName = "Hungary",
                    Capital = "Budapest"
                },
                new Country
                {
                    CountryName = "Iceland",
                    Capital = "Reykjavik"
                },
                new Country
                {
                    CountryName = "Ireland",
                    Capital = "Dublin"
                },
                new Country
                {
                    CountryName = "Italy",
                    Capital = "Rome"
                },
                new Country
                {
                    CountryName = "Kosovo",
                    Capital = "Pristina"
                },
                new Country
                {
                    CountryName = "Latvia",
                    Capital = "Riga"
                },
                new Country
                {
                    CountryName = "Liechtenstein",
                    Capital = "Vaduz"
                },
                new Country
                {
                    CountryName = "Lithuania",
                    Capital = "Vilnius"
                },
                new Country
                {
                    CountryName = "Luxembourg",
                    Capital = "Luxembourg"
                },
                new Country
                {
                    CountryName = "Macedonia",
                    Capital = "Skopje"
                },
                new Country
                {
                    CountryName = "Malta",
                    Capital = "Malta"
                },
                new Country
                {
                    CountryName = "Moldova",
                    Capital = "Chisinau"
                },
                new Country
                {
                    CountryName = "Monaco",
                    Capital = "Monaco"
                },
                new Country
                {
                    CountryName = "Montenegro",
                    Capital = "Podgorica"
                },
                new Country
                {
                    CountryName = "Netherlands",
                    Capital = "Amsterdam"
                },
                new Country
                {
                    CountryName = "Norway",
                    Capital = "Oslo"
                },
                new Country
                {
                    CountryName = "Poland",
                    Capital = "Warsaw"
                },
                new Country
                {
                    CountryName = "Portugal",
                    Capital = "Lisbon"
                },
                new Country
                {
                    CountryName = "Romania",
                    Capital = "Bucharest"
                },
                new Country
                {
                    CountryName = "Russia",
                    Capital = "Moscow"
                },
                new Country
                {
                    CountryName = "San Marino",
                    Capital = "San Marino"
                },
                new Country
                {
                    CountryName = "Serbia",
                    Capital = "Belgrade"
                },
                new Country
                {
                    CountryName = "Slovakia",
                    Capital = "Bratislava"
                },
                new Country
                {
                    CountryName = "Slovenia",
                    Capital = "Ljubljana"
                },
                new Country
                {
                    CountryName = "Spain",
                    Capital = "Madrid"
                },
                new Country
                {
                    CountryName = "Sweden",
                    Capital = "Stockholm"
                },
                new Country
                {
                    CountryName = "Switzerland",
                    Capital = "Bern"
                },
                  new Country
                {
                    CountryName = "Turkey",
                    Capital = "Ankara"
                },
                new Country
                {
                    CountryName = "Ukraine",
                    Capital = "Kyiv"
                },
                new Country
                {
                    CountryName = "United Kingdom",
                    Capital = "London"
                },
                new Country
                {
                    CountryName = "Vatican City",
                    Capital = "Vatican City"
                },

            };

            #endregion

            return CountryList;
        }

        private static List<PriceRange> BuildPriceRangesList()
        {
            List<PriceRange> PriceRangesList = new List<PriceRange>
            {
                new PriceRange
                {
                    PriceRangeValue = "0 - 5,000"
                },
                new PriceRange
                {
                    PriceRangeValue = "5,000 -10,000"
                },
                new PriceRange
                {
                    PriceRangeValue = "10,000 - 15,000"
                },
                new PriceRange
                {
                    PriceRangeValue = "15,000 - 20,000"
                },
                new PriceRange
                {
                    PriceRangeValue = "20,000 - 25,000"
                },
                new PriceRange
                {
                    PriceRangeValue = "25,000 - 30,000"
                },
                new PriceRange
                {
                    PriceRangeValue = "30,000 - 35,000"
                },
                new PriceRange
                {
                    PriceRangeValue = "35,000 - 40,000"
                },
                new PriceRange
                {
                    PriceRangeValue = "40,000 +"
                }
            };

            return PriceRangesList;
        }

        private static List<RecommendedRider> BuildRecommendedRidersList()
        {
            List<RecommendedRider> RecommendedRidersList = new List<RecommendedRider>
            {
                new RecommendedRider
                {
                    Rider = "Children"
                },
                new RecommendedRider
                {
                    Rider = "Junior"
                },
                new RecommendedRider
                {
                    Rider = "Amateur"
                },
                new RecommendedRider
                {
                    Rider = "Experienced Rider"
                },
                new RecommendedRider
                {
                    Rider = "Beginner"
                },
                new RecommendedRider
                {
                    Rider = "Young Rider"
                }
            };

            return RecommendedRidersList;
        }

        private static List<HorseAbility> BuildHorseAbilitiesList()
        {
            List<HorseAbility> HorseAbilitiesList = new List<HorseAbility>
            {
                new HorseAbility
                {
                    Ability = "Show Jumping"
                },
                new HorseAbility
                {
                    Ability = "Dressage"
                },
                new HorseAbility
                {
                    Ability = "Eventing"
                },
                new HorseAbility
                {
                    Ability = "Endurance"
                },
                new HorseAbility
                {
                    Ability = "Driving"
                },
                new HorseAbility
                {
                    Ability = "Leisure"
                },
                new HorseAbility
                {
                    Ability = "Foals"
                }
            };

            return HorseAbilitiesList;
        }

        private static List<Client> BuildClientsList()
        {

            List<Client> ClientsList = new List<Client>
            {
                new Client
                {   Id = "Horsespot_Web_Angular2",
                    Secret= Helper.GetHash("angular2-horsespot-application"),
                    Name = "Angular2 front-end horsespot application",
                    ApplicationType =  ApplicationTypes.Client_JavaScriptApplication,
                    Active = true,
                    RefreshTokenLifeTime = 7200,
                    AllowedOrigin = "*"
                },
            };

            return ClientsList;
        }
    }
}