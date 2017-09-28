using System;
using System.Configuration;

namespace HorseSpot.Infrastructure.Constants
{
    public static class ApplicationConstants
    {
        //DAL Constants
        public static int AdsPerPage = Convert.ToInt16(ConfigurationManager.AppSettings["AdsPerPage"]);
        public const int DefaultSortDirection = 1;
        public const string UserRole = "User";

        //BLL Constants
        public const int MaximumFileToUpload = 6;
        public static string DefaultProfilePhoto = ConfigurationManager.AppSettings["DefaultProfilePhotoName"];
        public const string ADMIN = "Admin";

        //Sort
        public const string SortDatePosted = "DatePosted";
        public const string SortAge = "Age";
        public const string SortHeight = "Height";
        public const string SortPrice = "Price";
        public const string SortViews = "Views";
    }
}
