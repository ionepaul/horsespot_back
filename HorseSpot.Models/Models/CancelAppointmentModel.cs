using System.ComponentModel.DataAnnotations;

namespace HorseSpot.Models.Models
{
    public class CancelAppointmentModel
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string FeedbackMessage { get; set; }

        [Required]
        public string AdOwnerId { get; set; }

        [Required]
        public string InitiatorId { get; set; }

        [Required]
        public bool OwnerCanceled { get; set; }

        [Required]
        public string AdvertismentTitle { get; set; }
    }
}
