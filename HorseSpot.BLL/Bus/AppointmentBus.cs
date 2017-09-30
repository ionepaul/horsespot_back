using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using HorseSpot.BLL.Converters;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.MailService;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Infrastructure.Validators;
using HorseSpot.Models.Enums;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Bus
{
    public class AppointmentBus : IAppointmentBus
    {
        #region Local Variables

        private IAppointmentDao _iAppointmentDao;
        private IUserDao _iUserDao;
        private IMailerService _iMailerService;
        private IHorseAdDao _iHorseAdDao;

        #endregion

        #region Constructor

        public AppointmentBus(IAppointmentDao iAppointmentDao, IUserDao iUserDao, IMailerService iMailerService, 
                              IHorseAdDao iHorseAdDao)
        {
            _iAppointmentDao = iAppointmentDao;
            _iUserDao = iUserDao;
            _iMailerService = iMailerService;
            _iHorseAdDao = iHorseAdDao;
        }

        #endregion

        #region Public Methods

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
