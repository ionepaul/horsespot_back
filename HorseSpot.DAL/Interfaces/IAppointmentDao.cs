using HorseSpot.DAL.Entities;
using System.Collections.Generic;

namespace HorseSpot.DAL.Interfaces
{
    public interface IAppointmentDao : IDao<Appointment>
    {
        IEnumerable<Appointment> GetUserAppointments(string userId);

        IEnumerable<Appointment> GetAppointmentsComingInTwoDays();

        IEnumerable<Appointment> GetDoneAppointments();

        IEnumerable<Appointment> GetUnseenAppointments(string userId);

        void UpdateAppointment(Appointment appointment);

        IEnumerable<Appointment> GetAppointmentsByHorseAdvertismentId(string horseAdId);
    }
}
