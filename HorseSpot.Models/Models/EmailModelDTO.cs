using System.ComponentModel.DataAnnotations;

namespace HorseSpot.Models.Models
{
    public class EmailModelDTO
    {
        [Required]
        public string Sender { get; set; }

        [Required]
        public string Receiver { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string SenderName { get; set; }

        public string HorseAdTitle { get; set; }

        [Required]
        public string ReceiverFirstName { get; set; }
    }
}
