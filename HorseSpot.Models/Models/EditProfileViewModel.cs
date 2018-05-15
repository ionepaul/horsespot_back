namespace HorseSpot.Models.Models
{
    public class EditProfileViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public bool NewsletterSubscription;

        public bool TermsAccepted;

        public bool DisplayEmail;

        public bool DisplayPhoneNumber;
    }
}
