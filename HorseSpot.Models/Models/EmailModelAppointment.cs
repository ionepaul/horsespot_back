using System;

namespace HorseSpot.Models.Models
{
    public class EmailModelAppointment
    {
        public string Sender { get; set; }

        public string SenderName { get; set; }

        public string Receiver { get; set; }

        public string ReceiverFirstName { get; set; }

        public string ReceiverLastName { get; set; }

        public string AppointmentLink { get; set; }

        public string AppointmentTitle { get; set; }

        public string UserWhoInitiatedEmail { get; set; }

        public string UserWhoInitiatedPhoneNumber { get; set; }

        public string AdOwnerEmail { get; set; }

        public string AdOwnerPhoneNumber { get; set; }

        public DateTimeOffset AppointmentDate { get; set; }

        public string EmailMessage { get; set; }

        public string EmailSubject { get; set; }

        public string EmailTemplatePath { get; set; }

        public string CancelFeedback { get; set; }

        public string AppointmentLocation { get; set; }

        public string UserWhoInitiatedFullName { get; set; }

        public string AdOwnerFullName { get; set; }
    }
}
