namespace HorseSpot.Models.Models
{
    public class UserDTO
    {
        public string Id { get; set; }

        public string FirstName{ get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string ImagePath { get; set; }

        public string Email { get; set; }

        public bool DisplayEmail { get; set; }

        public bool DisplayPhoneNumber { get; set; }
    }
}
