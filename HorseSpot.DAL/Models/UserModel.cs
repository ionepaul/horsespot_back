using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace HorseSpot.DAL.Models
{
    public class UserModel : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<bool> NewsletterSubscription { get; set; }
        public string ImagePath { get; set; }
    }
}