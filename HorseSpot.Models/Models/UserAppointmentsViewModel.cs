using System.Collections.Generic;

namespace HorseSpot.Models.Models
{
    public class UserAppointmentsViewModel
    {
        public UserAppointmentsViewModel()
        {
            UserPendingWhenInitiator = new List<AppointmentDTO>();
            UserPendingWhenOwner = new List<AppointmentDTO>();
            UserUpcomingWhenInitiator = new List<AppointmentDTO>();
            UserUpcomingWhenOwner = new List<AppointmentDTO>();
            UnseenAppointmentIds = new List<int>();
        }

        public IList<AppointmentDTO> UserPendingWhenOwner { get; set; }

        public IList<AppointmentDTO> UserPendingWhenInitiator { get; set; }

        public IList<AppointmentDTO> UserUpcomingWhenOwner { get; set; }

        public IList<AppointmentDTO> UserUpcomingWhenInitiator { get; set; }

        public IList<int> UnseenAppointmentIds { get; set; }
    }
}
