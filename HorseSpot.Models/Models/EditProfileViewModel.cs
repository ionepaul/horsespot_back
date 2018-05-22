namespace HorseSpot.Models.Models
{
    public class EditProfileViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public bool NewsletterSubscription { get; set; }

        public bool TermsAccepted { get; set; }

        public bool DisplayEmail { get; set; }

        public bool DisplayPhoneNumber { get; set; }

        public bool PrivacyPolicyAccepted { get; set; }
    }
}
