using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HorseSpot.DAL.Entities
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Status { get; set; }
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
        public string Title { get; set; }
        [Required]
        public bool IsDatePassed { get; set; }
        [Required]
        public bool SeenByOwner { get; set; }
        [Required]
        public bool SeenByInitiator { get; set; }
        [Required]
        public bool IsCanceled { get; set; }

        [ForeignKey("AdvertismentOwnerId")]
        public virtual UserModel AdOwner { get; set; }
        [ForeignKey("InitiatorId")]
        public virtual UserModel Initiator { get; set; }
        [ForeignKey("AdvertismentId")]
        public virtual HorseAd HorseAd { get; set; }
    }
}
