using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HorseSpot.Models.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        public bool? NewsletterSubscription { get; set; }

        public string RoleName { get; set; }
    }
}
