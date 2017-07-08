using HorseSpot.Models.Models;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Models;

namespace HorseSpot.BLL.Converters
{
    /// <summary>
    /// Static class used to map the appointment database model to aplication view model an vice-versa
    /// </summary>
    public static class AppointmentConverter
    {
        /// <summary>
        /// Converts appointment database model to appointment data transfer object
        /// </summary>
        /// <param name="appointment">Appointment Model</param>
        /// <param name="horseAd">Horse Ad Model</param>
        /// <returns>Appointment Data Transfer Object</returns>
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

        /// <summary>
        /// Converts appponintment data transfer object to appointment database model
        /// </summary>
        /// <param name="appointmentDTO">Appointment DTO Model</param>
        /// <returns>Apppointment Database Model</returns>
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
