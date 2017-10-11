﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseSpot.Api.Utils
{
    public static class ExternalAuthConstants
    {
        public static class Providers
        {
            public const string Facebook = "Facebook";
            public const string Google = "Google";
            public const string Twitter = "Twitter";
            public const string FacebookVerifyTokenEndPoint = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}";
            public const string GoogleVerifyTokenEndPoint = "https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}";
            public const string ObtainAppTokenUrl = "https://developers.facebook.com/tools/accesstoken/";
        }

        public static class CustomClaims
        {
            public const string Email = "Email";
            public const string FirstName = "FirstName";
            public const string LastName = "LastName";
            public const string ImageUrl = "ImageUrl";
            public const string ExternalAccessToken = "ExternalAccessToken";
        }
    }
}