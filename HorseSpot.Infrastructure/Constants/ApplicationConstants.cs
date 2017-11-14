using System;
using System.Configuration;

namespace HorseSpot.Infrastructure.Constants
{
    public static class ApplicationConstants
    {
        public static int AdsPerPage = Convert.ToInt16(ConfigurationManager.AppSettings["AdsPerPage"]);
        public const int DefaultSortDirection = 1;
        public const string UserRole = "User";

        public static string DefaultProfilePhoto = ConfigurationManager.AppSettings["DefaultProfilePhotoName"];
        public const string ADMIN = "Admin";

        public const string LatestDictionaryShowJumpingKey = "_showJumping";
        public const string LatestDictionaryDressageKey = "_dressage";
        public const string LatestDictionaryEventingKey = "_eventing";
        public const string LatestDictionaryEnduranceKey = "_endurance";

        public const int UI_MAX_PRICE = 100000;
    }
}
