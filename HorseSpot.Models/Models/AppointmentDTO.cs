using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HorseSpot.Models.Models
{
    public class AppointmentDTO
    { 
        public int Id { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        public int AdvertismentId { get; set; }

        [Required]
        public string AdvertismentOwnerId { get; set; }

        [Required]
        public string InitiatorId { get; set; }

        [Required]
        public bool IsAccepted { get; set; }

        [Required]
        public string STATUS { get; set; }

        public string AdvertismentTitle { get; set; }

        public string AdvertismentLocation { get; set; }

        /* OWNER DETAILS */

        public string OwnerFullName { get; set; }

        public string OwnerEmail { get; set; }

        public string OwnerPhoneNumber { get; set; }

        /* INITIATOR DETAILS */

        public string InitiatorFullName { get; set; }
        
        public string InitiatorEmail { get; set; }

        public string InitiatorPhoneNumber { get; set; }

        /* NOTIFICATIONS */

        public string Messeage { get; set; }
     
        public string NotificationBarMesseage { get; set; }

        public IEnumerable<int> SeenAppointmentsIds { get; set; }

        public string UserWhoSeenId { get; set; }

        public bool AllSeen { get; set; }
    }
}
