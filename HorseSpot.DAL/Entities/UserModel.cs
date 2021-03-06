﻿using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HorseSpot.DAL.Entities
{
    public class UserModel : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<bool> NewsletterSubscription { get; set; }
        public string ImagePath { get; set; }
        public bool TermsAccepted { get; set; }
        public bool PrivacyPolicyAccepted { get; set; }
        public bool DisplayEmail { get; set; }
        public bool DisplayPhoneNumber { get; set; }

        public virtual ICollection<HorseAd> HorseAds { get; set; }
        public virtual ICollection<UserFavoriteHorseAd> FavoriteHorseAds { get; set; }
    }
}