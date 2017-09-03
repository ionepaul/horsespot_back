using HorseSpot.Models.Models;
using System;
using System.Collections.Generic;

namespace HorseSpot.BLL.Interfaces
{
    public interface IAppointmentBus
    {
        AppointmentDTO MakeAppointment(AppointmentDTO appointmentDTO);

        AppointmentDTO UpdateAppointmentDateChangedByOwner(AppointmentDTO appointmentDTO);

        AppointmentDTO UpdateAppointmentDateChangedByInitiator(AppointmentDTO appointmentDTO);

        AppointmentDTO UpdateAppointmentAcceptedByOwner(AppointmentDTO appointmentDTO);

        AppointmentDTO UpdateAppointmentAcceptedByInitiator(AppointmentDTO appointmentDTO);

        void CancelAppointment(CancelAppointmentModel cancelAppointmentModel);

        UserAppointmentsViewModel GetUserAppointmentsViewModel(string userId);

        IEnumerable<AppointmentDTO> GetUnseenAppointmentsForUser(string userId);

        void SetAppointmentsAsSeen(string userId, IEnumerable<int> unseenAppointmentsId);

        Boolean CheckUserId(string userId);

        void SendEmailForAppointmentsComingInTwoDays();

        void SendEmailForResolvedAppointments();
    }
}
