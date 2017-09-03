using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HorseSpot.Models.Models
{
    public class ContactPageEmailModel
    {
        [Required]
        public string Sender { get; set; }

        [Required]
        public string SenderName { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
