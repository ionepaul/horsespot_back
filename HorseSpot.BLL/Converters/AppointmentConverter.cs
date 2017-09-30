using HorseSpot.DAL.Entities;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Converters
{
    public static class AppointmentConverter
    {
        public static AppointmentDTO FromAppointmentToAppointmentDTO(Appointment appointment, HorseAd horseAd)
        {
            return new AppointmentDTO
            {
                Id = appointment.Id,
                AdvertismentId = appointment.AdvertismentId,
                AdvertismentTitle = appointment.Title,
                AdvertismentLocation = horseAd.Address.Country + ", " + horseAd.Address.City + ", " + horseAd.Address.Street,
                AppointmentDateTime = appointment.AppointmentDateTime,
                AdvertismentOwnerId = appointment.AdvertismentOwnerId,
                OwnerEmail = appointment.AdOwner.Email,
                OwnerFullName = appointment.AdOwner.FirstName + " " + appointment.AdOwner.LastName,
                OwnerPhoneNumber = appointment.AdOwner.PhoneNumber,
                InitiatorFullName = appointment.Initiator.FirstName + " " + appointment.Initiator.LastName,
                InitiatorId = appointment.Initiator.Id,
                InitiatorEmail = appointment.Initiator.Email,
                InitiatorPhoneNumber = appointment.Initiator.PhoneNumber,
                IsAccepted = appointment.IsAccepted,
                STATUS = appointment.Status
            };
        }

        public static Appointment FromAppointmentDTOToAppointment(AppointmentDTO appointmentDTO)
        {
            return new Appointment
            {
                AdvertismentId = appointmentDTO.AdvertismentId,
                AppointmentDateTime = appointmentDTO.AppointmentDateTime,
                AdvertismentOwnerId = appointmentDTO.AdvertismentOwnerId,
                InitiatorId = appointmentDTO.InitiatorId,
                IsAccepted = appointmentDTO.IsAccepted,
                Status = appointmentDTO.STATUS,
                Title = appointmentDTO.AdvertismentTitle,
                IsDatePassed = false,
                SeenByOwner = false,
                SeenByInitiator = true
            };
        }
    }
}
