using HorseSpot.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using HorseSpot.Models.Models;
using HorseSpot.DAL.Interfaces;
using HorseSpot.BLL.Converters;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.Validators;
using HorseSpot.DAL.Entities;
using System.Configuration;
using HorseSpot.Infrastructure.MailService;
using HorseSpot.DAL.Models;
using HorseSpot.Models.Enums;
using HorseSpot.Infrastructure.Resources;

namespace HorseSpot.BLL.Bus
{
    public class AppointmentBus : IAppointmentBus
    {
        #region Local Variables

        private IAppointmentDao _iAppointmentDao;
        private IUserDao _iUserDao;
        private IMailerService _iMailerService;
        private IHorseAdDao _iHorseAdDao;
        private ICountryDao _iCountryDao;

        #endregion

        #region Constructor

        /// <summary>
        /// AppointmentBus Constructor
        /// </summary>
        /// <param name="iAppointmentDao">Appointment Dao Interface</param>
        /// <param name="iUserDao">User Dao Interface</param>
        /// <param name="iMailerService">Mailer Service Interface</param>
        /// <param name="iHorseAdDao">HorseAd Dao Interface</param>
        /// <param name="iCountryDao">Country Dao Interface</param>
        public AppointmentBus(IAppointmentDao iAppointmentDao, IUserDao iUserDao, IMailerService iMailerService, 
                              IHorseAdDao iHorseAdDao, ICountryDao iCountryDao)
        {
            _iAppointmentDao = iAppointmentDao;
            _iUserDao = iUserDao;
            _iMailerService = iMailerService;
            _iHorseAdDao = iHorseAdDao;
            _iCountryDao = iCountryDao;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates an appointment and sends email to adverstisment owner
        /// </summary>
        /// <param name="appointmentDTO">Appointment Model</param>
        /// <returns>Created Appointment</returns>
        public AppointmentDTO MakeAppointment(AppointmentDTO appointmentDTO)
        {
            var validatedAppointmentDTO = ValidateAndSetAppointment(appointmentDTO, AppointmentStatus.CREATED);

            var appointment = AppointmentConverter.FromAppointmentDTOToAppointment(validatedAppointmentDTO);

            _iAppointmentDao.Add(appointment);

            var owner = _iUserDao.FindUserById(appointmentDTO.AdvertismentOwnerId);
            var initiator = _iUserDao.FindUserById(appointmentDTO.InitiatorId);

            EmailModelAppointment emailModel = new EmailModelAppointment
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                SenderName = initiator.FirstName + " " + initiator.LastName,
                Receiver = owner.Email,
                ReceiverFirstName = owner.FirstName,
                ReceiverLastName = owner.LastName,
                EmailSubject = EmailSubjects.NewAppointment,
                AppointmentTitle = appointmentDTO.AdvertismentTitle,
                AppointmentDate = appointmentDTO.AppointmentDateTime,
                EmailTemplatePath = EmailTemplatesPath.MakeAppointmentTemplate
            };

            _iMailerService.SendMailAppointments(emailModel);

            return CreateAppoinmentNotification(appointment, owner, initiator);
        }

        /// <summary>
        /// Gets the unseen appointments for a user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>List of unseen appointments</returns>
        public IEnumerable<AppointmentDTO> GetUnseenAppointmentsForUser(string userId)
        {
            var notifications = new List<AppointmentDTO>();
            var unseenByUser = _iAppointmentDao.GetUnseenAppointments(userId);

            foreach(var unseenAppointment in unseenByUser)
            {
                var notif = CreateAppoinmentNotification(unseenAppointment, unseenAppointment.AdOwner, unseenAppointment.Initiator);
                notifications.Add(notif);
            }

            return notifications.AsEnumerable();
        }

        /// <summary>
        /// Set the unseen appointments
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="unseenAppointmentsId">List of appointment ids</param>
        public void SetAppointmentsAsSeen(string userId, IEnumerable<int> unseenAppointmentsId)
        {
            foreach(var id in unseenAppointmentsId)
            {
                var appointment = _iAppointmentDao.GetById(id);
                
                if (appointment.AdvertismentOwnerId == userId)
                {
                    appointment.SeenByOwner = true;
                }

                if (appointment.InitiatorId == userId)
                {
                    appointment.SeenByInitiator = true;
                }

                _iAppointmentDao.UpdateAppointment(appointment);
            }
        }

        /// <summary>
        /// Updates the appointment date when changed by owner and sends email to the appointment initiator
        /// </summary>
        /// <param name="appointmentDTO">Appointment Model</param>
        /// <returns>Updated Appointment Model</returns>
        public AppointmentDTO UpdateAppointmentDateChangedByOwner(AppointmentDTO appointmentDTO)
        {
            var appointment = ValidateAndSetUpdateAppointment(appointmentDTO, AppointmentStatus.DATE_CHANGED_BY_OWNER);
            appointment.SeenByOwner = true;
            appointment.SeenByInitiator = false;

            var owner = _iUserDao.FindUserById(appointment.AdvertismentOwnerId);
            var initiator = _iUserDao.FindUserById(appointment.InitiatorId);

            EmailModelAppointment emailModel = new EmailModelAppointment
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                SenderName = owner.FirstName + " " + owner.LastName,
                Receiver = initiator.Email,
                ReceiverFirstName = initiator.FirstName,
                ReceiverLastName = initiator.LastName,
                EmailSubject = EmailSubjects.OwnerChangedDate,
                AppointmentTitle = appointmentDTO.AdvertismentTitle,
                AppointmentDate = appointmentDTO.AppointmentDateTime,
                EmailTemplatePath = EmailTemplatesPath.OwnerChangedDateTemplate
            };

            _iMailerService.SendMailAppointments(emailModel);
            _iAppointmentDao.UpdateAppointment(appointment);

            return CreateAppoinmentNotification(appointment, owner, initiator);
        }

        /// <summary>
        /// Updates the appointment date when changed by initiator and sends email to the advertisment owner
        /// </summary>
        /// <param name="appointmentDTO">Appointment Model</param>
        /// <returns>Updated Appointment Model</returns>
        public AppointmentDTO UpdateAppointmentDateChangedByInitiator(AppointmentDTO appointmentDTO)
        {
            var appointment = ValidateAndSetUpdateAppointment(appointmentDTO, AppointmentStatus.DATE_CHANGED_BY_INITIATOR);
            appointment.SeenByOwner = false;
            appointment.SeenByInitiator = true;

            var owner = _iUserDao.FindUserById(appointment.AdvertismentOwnerId);
            var initiator = _iUserDao.FindUserById(appointment.InitiatorId);

            EmailModelAppointment emailModel = new EmailModelAppointment
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                SenderName = initiator.FirstName + " " + initiator.LastName,
                Receiver = owner.Email,
                ReceiverFirstName = owner.FirstName,
                ReceiverLastName = owner.LastName,
                EmailSubject = EmailSubjects.InitiatorChangedDate,
                AppointmentTitle = appointmentDTO.AdvertismentTitle,
                AppointmentDate = appointmentDTO.AppointmentDateTime,
                EmailTemplatePath = EmailTemplatesPath.InitiatorChangedDateTemplate
            };

            _iMailerService.SendMailAppointments(emailModel);
            _iAppointmentDao.UpdateAppointment(appointment);

            return CreateAppoinmentNotification(appointment, owner, initiator);
        }

        /// <summary>
        /// Updates the appointment when accepted by owner and sends email to the appointment initiator
        /// </summary>
        /// <param name="appointmentDTO">Appointment Model</param>
        /// <returns>Updated Appointment Model</returns>
        public AppointmentDTO UpdateAppointmentAcceptedByOwner(AppointmentDTO appointmentDTO)
        {
            var appointment = ValidateAndSetUpdateAppointment(appointmentDTO, AppointmentStatus.ACCEPTED_BY_OWNER);
            appointment.SeenByOwner = true;
            appointment.SeenByInitiator = false;

            var owner = _iUserDao.FindUserById(appointment.AdvertismentOwnerId);
            var initiator = _iUserDao.FindUserById(appointment.InitiatorId);

            EmailModelAppointment emailModel = new EmailModelAppointment
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                SenderName = owner.FirstName + " " + owner.LastName,
                Receiver = initiator.Email,
                ReceiverFirstName = initiator.FirstName,
                ReceiverLastName = initiator.LastName,
                EmailSubject = EmailSubjects.AppointmentAccepted,
                AppointmentTitle = appointmentDTO.AdvertismentTitle,
                AppointmentDate = appointmentDTO.AppointmentDateTime,
                EmailTemplatePath = EmailTemplatesPath.AppointmentAcceptedTemplate
            };

            _iMailerService.SendMailAppointments(emailModel);
            _iAppointmentDao.UpdateAppointment(appointment);

            return CreateAppoinmentNotification(appointment, owner, initiator);
        }

        /// <summary>
        /// Updates the appointment when accepted by initiator and sends email to the advertisment owner
        /// </summary>
        /// <param name="appointmentDTO">Appointment Model</param>
        /// <returns>Updated Appointment Model</returns>
        public AppointmentDTO UpdateAppointmentAcceptedByInitiator(AppointmentDTO appointmentDTO)
        {
            var appointment = ValidateAndSetUpdateAppointment(appointmentDTO, AppointmentStatus.ACCEPTED_BY_INITIATOR);
            appointment.SeenByOwner = false;
            appointment.SeenByInitiator = true;

            var owner = _iUserDao.FindUserById(appointment.AdvertismentOwnerId);
            var initiator = _iUserDao.FindUserById(appointment.InitiatorId);

            EmailModelAppointment emailModel = new EmailModelAppointment
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                SenderName = initiator.FirstName + " " + initiator.LastName,
                Receiver = owner.Email,
                ReceiverFirstName = owner.FirstName,
                ReceiverLastName = owner.LastName,
                EmailSubject = EmailSubjects.AppointmentAccepted,
                AppointmentTitle = appointmentDTO.AdvertismentTitle,
                AppointmentDate = appointmentDTO.AppointmentDateTime,
                EmailTemplatePath = EmailTemplatesPath.AppointmentAcceptedTemplate
            };

            _iMailerService.SendMailAppointments(emailModel);
            _iAppointmentDao.UpdateAppointment(appointment);

            return CreateAppoinmentNotification(appointment, owner, initiator);
        }

        /// <summary>
        /// Get all appointments for a user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Model containig lists of all appointments types</returns>
        public UserAppointmentsViewModel GetUserAppointmentsViewModel(string userId)
        {
            var userAppointmentsViewModel = new UserAppointmentsViewModel();

            var userAppointments = _iAppointmentDao.GetUserAppointments(userId);

            foreach (var userAppointment in userAppointments)
            {
                var horseAd = _iHorseAdDao.GetById(userAppointment.AdvertismentId);
                var appointmentDTO = AppointmentConverter.FromAppointmentToAppointmentDTO(userAppointment, horseAd);

                if (userAppointment.InitiatorId == userId && userAppointment.IsAccepted == false)
                {
                    userAppointmentsViewModel.UserPendingWhenInitiator.Add(appointmentDTO);
                    if (!userAppointment.SeenByInitiator)
                    {
                        userAppointmentsViewModel.UnseenAppointmentIds.Add(userAppointment.Id);
                    }
                }

                if (userAppointment.InitiatorId == userId && userAppointment.IsAccepted == true)
                {
                    userAppointmentsViewModel.UserUpcomingWhenInitiator.Add(appointmentDTO);
                    if (!userAppointment.SeenByInitiator)
                    {
                        userAppointmentsViewModel.UnseenAppointmentIds.Add(userAppointment.Id);
                    }
                }

                if (userAppointment.AdvertismentOwnerId == userId && userAppointment.IsAccepted == false)
                {
                    userAppointmentsViewModel.UserPendingWhenOwner.Add(appointmentDTO);
                    if (!userAppointment.SeenByOwner)
                    {
                        userAppointmentsViewModel.UnseenAppointmentIds.Add(userAppointment.Id);
                    }
                }

                if (userAppointment.AdvertismentOwnerId == userId && userAppointment.IsAccepted == true)
                {
                    userAppointmentsViewModel.UserUpcomingWhenOwner.Add(appointmentDTO);
                    if (!userAppointment.SeenByOwner)
                    {
                        userAppointmentsViewModel.UnseenAppointmentIds.Add(userAppointment.Id);
                    }
                }
            }

            return userAppointmentsViewModel;
        }

        /// <summary>
        /// Cancel an appointment
        /// </summary>
        /// <param name="cancelAppointmentModel">Cancel Appointment Model</param>
        public void CancelAppointment(CancelAppointmentModel cancelAppointmentModel)
        {
            var appointment = _iAppointmentDao.GetById(cancelAppointmentModel.AppointmentId);

            var initiator = appointment.Initiator;
            var owner = appointment.AdOwner;

            _iAppointmentDao.Delete(appointment);

            if (cancelAppointmentModel.OwnerCanceled)
            {
                EmailModelAppointment emailModel = new EmailModelAppointment
                {
                    Sender = ConfigurationManager.AppSettings["AdminEmail"],
                    SenderName = owner.FirstName + " " + owner.LastName,
                    Receiver = initiator.Email,
                    ReceiverFirstName = initiator.FirstName,
                    ReceiverLastName = initiator.LastName,
                    EmailSubject = EmailSubjects.AppointmentCanceled,
                    CancelFeedback = cancelAppointmentModel.FeedbackMessage,
                    AppointmentTitle = cancelAppointmentModel.AdvertismentTitle,
                    EmailTemplatePath = EmailTemplatesPath.AppointmentCanceledTemplate
                };

                _iMailerService.SendMailAppointments(emailModel);
            }
            else
            {
                EmailModelAppointment emailModel = new EmailModelAppointment
                {
                    Sender = ConfigurationManager.AppSettings["AdminEmail"],
                    SenderName = initiator.FirstName + " " + initiator.LastName,
                    Receiver = owner.Email,
                    ReceiverFirstName = owner.FirstName,
                    ReceiverLastName = owner.LastName,
                    EmailSubject = EmailSubjects.AppointmentCanceled,
                    CancelFeedback = cancelAppointmentModel.FeedbackMessage,
                    AppointmentTitle = cancelAppointmentModel.AdvertismentTitle,
                    EmailTemplatePath = EmailTemplatesPath.AppointmentCanceledTemplate
                };

                _iMailerService.SendMailAppointments(emailModel);
            }

        }

        /// <summary>
        /// Check for upcoming appointments in two days and send emails to notify participants
        /// </summary>
        public void SendEmailForAppointmentsComingInTwoDays()
        {
            var upcomingAppointments = _iAppointmentDao.GetAppointmentsComingInTwoDays();

            foreach (var appointment in upcomingAppointments)
            {
                var horseAd = _iHorseAdDao.GetById(appointment.AdvertismentId);
                var owner = _iUserDao.FindUserById(appointment.AdvertismentOwnerId);
                var initiator = _iUserDao.FindUserById(appointment.InitiatorId);

                SendTwoDaysNotifierEmailOwner(owner, appointment, initiator, horseAd);
                SendTwoDaysNotifierEmailInitiator(initiator, appointment, owner, horseAd);
            }
        }

        /// <summary>
        /// Check for appointments that took place and ask the participants for feedback
        /// </summary>
        public void SendEmailForResolvedAppointments()
        {
            var resolvedAppointments = _iAppointmentDao.GetDoneAppointments();

            foreach (var appointment in resolvedAppointments)
            {
                appointment.IsDatePassed = true;

                if (appointment.IsAccepted)
                {
                    var horseAd = _iHorseAdDao.GetById(appointment.AdvertismentId);
                    var owner = _iUserDao.FindUserById(appointment.AdvertismentOwnerId);
                    var initiator = _iUserDao.FindUserById(appointment.InitiatorId);

                    SendEmailForDoneAppointmentToOwner(owner, horseAd);
                    SendEmailForDoneAppointmentToInitiator(initiator, horseAd);
                }
            }

            _iAppointmentDao.SaveChanges();
        }

        /// <summary>
        /// Check if user exists by id
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>True/False</returns>
        public bool CheckUserId(string userId)
        {
            if (_iUserDao.FindUserById(userId) != null)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates and set appointment current status
        /// </summary>
        /// <param name="appointmentDTO">Appointemnt Model</param>
        /// <param name="status">Status</param>
        /// <returns>Updated Appoinment or Exception if invalid data</returns>
        private AppointmentDTO ValidateAndSetAppointment(AppointmentDTO appointmentDTO, string status)
        {
            if (appointmentDTO == null)
            {
                throw new ValidationException(Resources.InvalidMakeAppRequest);
            }

            ValidationHelper.ValidateModelAttributes<AppointmentDTO>(appointmentDTO);

            if (appointmentDTO.AppointmentDateTime.ToUniversalTime() < DateTime.UtcNow)
            {
                throw new ValidationException(Resources.AppointmentCannotBeInPast);
            }

            var horseAd = _iHorseAdDao.GetById(appointmentDTO.AdvertismentId);

            if (horseAd == null)
            {
                throw new ValidationException(Resources.InvalidAdIdentifier);
            }

            var userInitiator = _iUserDao.FindUserById(appointmentDTO.InitiatorId);

            if (userInitiator == null)
            {
                throw new ValidationException(Resources.InvalidUserIdentifier);
            }

            appointmentDTO.STATUS = status;
        
            return appointmentDTO;
        }

        /// <summary>
        /// Validate and updates an appointment
        /// </summary>
        /// <param name="appointmentDTO">Appointment Model</param>
        /// <param name="status">Status</param>
        /// <returns>Updated appointment or Exception if invalid data</returns>
        private Appointment ValidateAndSetUpdateAppointment(AppointmentDTO appointmentDTO, string status)
        {
            var appointment = _iAppointmentDao.GetById(appointmentDTO.Id);

            if (appointmentDTO.AppointmentDateTime.ToUniversalTime() < DateTime.UtcNow)
            {
                throw new ValidationException(Resources.AppointmentCannotBeInPast);
            }

            appointment.IsAccepted = appointmentDTO.IsAccepted;
            appointment.Status = status;
            appointment.AppointmentDateTime = appointmentDTO.AppointmentDateTime;

            return appointment;
        }

        /// <summary>
        /// Create the notification message that will be send to end user
        /// </summary>
        /// <param name="appointment">Appointment Model</param>
        /// <param name="owner">Advertisment Owner User Model</param>
        /// <param name="initiator">Appointment Initiator User Model</param>
        /// <returns></returns>
        private AppointmentDTO CreateAppoinmentNotification(Appointment appointment, UserModel owner, UserModel initiator)
        {
            var appointmentNotif = new AppointmentDTO();
            switch (appointment.Status)
            {
                case AppointmentStatus.CREATED:
                    appointmentNotif.Messeage = "You have a new appointment from ";
                    appointmentNotif.NotificationBarMesseage = "You have a new appointment from " + initiator.FirstName + " " + initiator.LastName + " for your " + appointment.Title;
                    appointmentNotif.InitiatorFullName = initiator.FirstName + " " + initiator.LastName;
                    appointmentNotif.AppointmentDateTime = appointment.AppointmentDateTime;
                    break;
                case AppointmentStatus.DATE_CHANGED_BY_OWNER:
                    appointmentNotif.Messeage = "Appointment date changed by ";
                    appointmentNotif.NotificationBarMesseage = owner.FirstName + " " + owner.LastName + " asked for different date regarding " + appointment.Title;
                    appointmentNotif.InitiatorFullName = owner.FirstName + " " + owner.LastName;
                    appointmentNotif.AppointmentDateTime = appointment.AppointmentDateTime;
                    break;
                case AppointmentStatus.DATE_CHANGED_BY_INITIATOR:
                    appointmentNotif.Messeage = "Appointment date changed by ";
                    appointmentNotif.NotificationBarMesseage = initiator.FirstName + " " + initiator.LastName + " asked for different date regarding your " + appointment.Title;
                    appointmentNotif.InitiatorFullName = initiator.FirstName + " " + initiator.LastName;
                    appointmentNotif.AppointmentDateTime = appointment.AppointmentDateTime;
                    break;
                case AppointmentStatus.ACCEPTED_BY_OWNER:
                    appointmentNotif.Messeage = "Appointment accepted by ";
                    appointmentNotif.NotificationBarMesseage = "All set." + owner.FirstName + " " + owner.LastName + " accepted the appointment for " + appointment.Title;
                    appointmentNotif.InitiatorFullName = owner.FirstName + " " + owner.LastName;
                    appointmentNotif.AppointmentDateTime = appointment.AppointmentDateTime;
                    break;
                case AppointmentStatus.ACCEPTED_BY_INITIATOR:
                    appointmentNotif.Messeage = "Appointment accepted by ";
                    appointmentNotif.NotificationBarMesseage = "All set." + initiator.FirstName + " " + initiator.LastName + " accepted the appointment for your " + appointment.Title;
                    appointmentNotif.InitiatorFullName = initiator.FirstName + " " + initiator.LastName;
                    appointmentNotif.AppointmentDateTime = appointment.AppointmentDateTime;
                    break;
                default:
                    break;
            }

            return appointmentNotif;
        }

        /// <summary>
        /// Send email to notify owner that an appointment is coming in two days
        /// </summary>
        /// <param name="owner">Advertisment Owner User Model</param>
        /// <param name="appointment">Appointment Model</param>
        /// <param name="initiator">Appointment Initiator User Model</param>
        /// <param name="horseAd">Advertisment Model</param>
        private void SendTwoDaysNotifierEmailOwner(UserModel owner, Appointment appointment, UserModel initiator, HorseAd horseAd)
        {
            EmailModelAppointment emailModel = new EmailModelAppointment
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = owner.Email,
                ReceiverFirstName = owner.FirstName,
                ReceiverLastName = owner.LastName,
                EmailSubject = EmailSubjects.AppointmentComing,
                AppointmentTitle = horseAd.Title,
                AppointmentDate = appointment.AppointmentDateTime,
                AppointmentLocation = horseAd.Address.Country + ", " + horseAd.Address.City + ", " + horseAd.Address.Street,
                UserWhoInitiatedEmail = initiator.Email,
                UserWhoInitiatedPhoneNumber = initiator.PhoneNumber,
                UserWhoInitiatedFullName = initiator.FirstName + " " + initiator.LastName,
                EmailTemplatePath = EmailTemplatesPath.AppointmentComingToOwner
            };

            _iMailerService.SendMailAppointments(emailModel);
        }

        /// <summary>
        /// Send email to notify initiator that an appointment is coming in two days
        /// </summary>
        /// <param name="owner">Advertisment Owner User Model</param>
        /// <param name="appointment">Appointment Model</param>
        /// <param name="initiator">Appointment Initiator User Model</param>
        /// <param name="horseAd">Advertisment Model</param>
        private void SendTwoDaysNotifierEmailInitiator(UserModel initiator, Appointment appointment, UserModel owner, HorseAd horseAd)
        {
            EmailModelAppointment emailModel = new EmailModelAppointment
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = initiator.Email,
                ReceiverFirstName = initiator.FirstName,
                ReceiverLastName = initiator.LastName,
                EmailSubject = EmailSubjects.AppointmentComing,
                AppointmentTitle = horseAd.Title,
                AppointmentDate = appointment.AppointmentDateTime,
                AppointmentLocation = horseAd.Address.Country + ", " + horseAd.Address.City + ", " + horseAd.Address.Street,
                AdOwnerEmail = owner.FirstName,
                AdOwnerPhoneNumber = owner.LastName,
                AdOwnerFullName = owner.FirstName + " " + owner.LastName,
                EmailTemplatePath = EmailTemplatesPath.AppointmentComingToInitiator
            };

            _iMailerService.SendMailAppointments(emailModel);
        }
        
        /// <summary>
        /// Ask owner for feedback after an appointment took place
        /// </summary>
        /// <param name="owner">Advertisment Owner Model</param>
        /// <param name="horseAd">Advertisment Model</param>
        private void SendEmailForDoneAppointmentToOwner(UserModel owner, HorseAd horseAd)
        {
            EmailModelAppointment emailModel = new EmailModelAppointment
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = owner.Email,
                ReceiverFirstName = owner.FirstName,
                ReceiverLastName = owner.LastName,
                EmailSubject = EmailSubjects.HowWasTheAppointment,
                AppointmentTitle = horseAd.Title,
                EmailTemplatePath = EmailTemplatesPath.AppointmentDoneToOwnerTemplate
            };

            _iMailerService.SendMailAppointments(emailModel);
        }

        /// <summary>
        /// Ask initiator for feedback after an appointment took place
        /// </summary>
        /// <param name="owner">Advertisment Owner Model</param>
        /// <param name="horseAd">Advertisment Model</param>
        private void SendEmailForDoneAppointmentToInitiator(UserModel initiator, HorseAd horseAd)
        {
            EmailModelAppointment emailModel = new EmailModelAppointment
            {
                Sender = ConfigurationManager.AppSettings["AdminEmail"],
                Receiver = initiator.Email,
                ReceiverFirstName = initiator.FirstName,
                ReceiverLastName = initiator.LastName,
                EmailSubject = EmailSubjects.HowWasTheAppointment,
                AppointmentTitle = horseAd.Title,
                EmailTemplatePath = EmailTemplatesPath.AppointmentDoneToInitiatorTemplate
            };

            _iMailerService.SendMailAppointments(emailModel);
        }

        #endregion
    }
}
