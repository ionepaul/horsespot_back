namespace HorseSpot.Models.Models
{
    public class EmailModel
    {
        public string Sender { get; set; }

        public string SenderName { get; set; }

        public string Receiver { get; set; }

        public string ReceiverFirstName { get; set; }

        public string ReceiverLastName { get; set; }

        public string ValidateLink { get; set; }

        public string HorseAdLink { get; set; }

        public string HorseAdTitle { get; set; }

        public string TemporaryPassword { get; set; }

        public string EmailSubject { get; set; }

        public string EmailMessage { get; set; }

        public string EmailTemplatePath { get; set; }
    }
}
