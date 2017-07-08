using System.ComponentModel.DataAnnotations;

namespace HorseSpot.DAL.Entities
{
    public class Subscriber
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; }

        public Subscriber() { }

        public Subscriber(string email)
        {
            Email = email;
        }
    }
}
