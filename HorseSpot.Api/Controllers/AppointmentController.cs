using HorseSpot.BLL.Interfaces;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Models.Enums;
using HorseSpot.Models.Models;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HorseSpot.Api.Controllers
{
    public class AppointmentController : ApiController
    {
        private IAppointmentBus _iAppointmentBus;

        /// <summary>
        /// Appointment Controller Constructor
        /// </summary>
        /// <param name="iAppointmentBus">Appointment Bussines Logic Interface</param>
        public AppointmentController(IAppointmentBus iAppointmentBus)
        {
            _iAppointmentBus = iAppointmentBus;
        }

        /// <summary>
        /// API Interface to cancel appointment
        /// </summary>
        /// <param name="cancelAppointmentModel">Cancel Appointment Model</param>
        [Route("api/appointments/cancel")]
        [Authorize]
        [HttpPost]
        public void CancelAppointment([FromBody] CancelAppointmentModel cancelAppointmentModel)
        {
            _iAppointmentBus.CancelAppointment(cancelAppointmentModel);
        }

        /// <summary>
        /// API Interface to get apppointments for an user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Model containing of lists of all appointment types</returns>
        [Route("api/appointments/user/{userId}")]
        [Authorize]
        [HttpGet]
        public UserAppointmentsViewModel GetUpcomingAppointmentsOwner([FromUri] string userId)
        {
            return _iAppointmentBus.GetUserAppointmentsViewModel(userId);
        }

        /// <summary>
        /// Get unseen appointments for an user by id
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>List of appointments</returns>
        [Route("api/appointments/unseen/{userId}")]
        [Authorize]
        [HttpGet]
        public IEnumerable<AppointmentDTO> GetUnseenAppointmentsForUser([FromUri] string userId)
        {
            return _iAppointmentBus.GetUnseenAppointmentsForUser(userId);
        }

        /// <summary>
        /// Switch protocol to websocket if user is connected to application
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Switching Protocols or Exception</returns>
        [Route("api/appointment")]
        public HttpResponseMessage Get(string userId)
        {
            if (!_iAppointmentBus.CheckUserId(userId))
            {
                throw new ForbiddenException(Resources.WebSocketConnectionForbidden);
            }
            else
            {
                HttpContext.Current.AcceptWebSocketRequest(new AppointmentWebSocketHandler(userId, _iAppointmentBus));
                return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
            }
        }

        /// <summary>
        /// Class that handles web socket requests
        /// </summary>
        class AppointmentWebSocketHandler : WebSocketHandler
        {
            private static WebSocketCollection _clients = new WebSocketCollection();
            private IAppointmentBus _iAppointmentBus;
            private string _userId;

            /// <summary>
            /// Appointment Web Socket Handler Constructor
            /// </summary>
            /// <param name="userId">User Id to connect</param>
            /// <param name="iAppointmentBus">Appointment Bussines Logic Interface</param>
            public AppointmentWebSocketHandler(string userId, IAppointmentBus iAppointmentBus)
            {
                _iAppointmentBus = iAppointmentBus;
                _userId = userId;
            }

            /// <summary>
            /// Add clients on open
            /// </summary>
            public override void OnOpen()
            {
                _clients.Add(this);
            }

            /// <summary>
            /// Handles message exchanges
            /// </summary>
            /// <param name="message">Message Received</param>
            public override void OnMessage(string message)
            {
                try
                {
                    var appointmentDTO = JsonConvert.DeserializeObject<AppointmentDTO>(message);
                    var appointmentNotification = new AppointmentDTO();

                    switch (appointmentDTO.STATUS)
                    {
                        case AppointmentStatus.CREATED:
                            AppoinmentCreated(appointmentDTO);
                            break;
                        case AppointmentStatus.DATE_CHANGED_BY_INITIATOR:
                            AppointmentDateChangedByInitiator(appointmentDTO);
                            break;
                        case AppointmentStatus.DATE_CHANGED_BY_OWNER:
                            AppointmentDateChangedByOwner(appointmentDTO);
                            break;
                        case AppointmentStatus.ACCEPTED_BY_OWNER:
                            AppointmentAcceptedByOwner(appointmentDTO);
                            break;
                        case AppointmentStatus.ACCEPTED_BY_INITIATOR:
                            AppointmentAcceptedByInitiator(appointmentDTO);
                            break;
                        case AppointmentStatus.SET_SEEN_APPOINTMETS:
                            SetSeenAppointments(appointmentDTO);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    //Catch error when user who needs to be notified is not connected
                }
            }

            /// <summary>
            /// Send notification for appointment created to advertisment owner
            /// </summary>
            /// <param name="appointmentDTO">Appointment Model</param>
            public void AppoinmentCreated(AppointmentDTO appointmentDTO)
            {
                var appointmentNotification = _iAppointmentBus.MakeAppointment(appointmentDTO);
                var send = JsonConvert.SerializeObject(appointmentNotification);
                _clients.FirstOrDefault(r => ((AppointmentWebSocketHandler)r)._userId == appointmentDTO.AdvertismentOwnerId).Send(send);
            }

            /// <summary>
            /// If date change by initiator send notification for date changed to advertisment owner
            /// </summary>
            /// <param name="appointmentDTO">Appointment Model</param>
            public void AppointmentDateChangedByInitiator(AppointmentDTO appointmentDTO)
            {
                var appointmentNotification = _iAppointmentBus.UpdateAppointmentDateChangedByOwner(appointmentDTO);
                var send = JsonConvert.SerializeObject(appointmentNotification);
                _clients.FirstOrDefault(r => ((AppointmentWebSocketHandler)r)._userId == appointmentDTO.InitiatorId).Send(send);
            }

            /// <summary>
            /// If date change by owner send notification for date changed to appointment initiator
            /// </summary>
            /// <param name="appointmentDTO">Appointment Model</param>
            public void AppointmentDateChangedByOwner(AppointmentDTO appointmentDTO)
            {
                var appointmentNotification = _iAppointmentBus.UpdateAppointmentDateChangedByInitiator(appointmentDTO);
                var send = JsonConvert.SerializeObject(appointmentNotification);
                _clients.FirstOrDefault(r => ((AppointmentWebSocketHandler)r)._userId == appointmentDTO.AdvertismentOwnerId).Send(send);
            }

            /// <summary>
            /// If appointment accepted by owner send notification to initiator
            /// </summary>
            /// <param name="appointmentDTO">Appointment Model</param>
            public void AppointmentAcceptedByOwner(AppointmentDTO appointmentDTO)
            {
                var appointmentNotification = _iAppointmentBus.UpdateAppointmentAcceptedByOwner(appointmentDTO);
                var send = JsonConvert.SerializeObject(appointmentNotification);
                _clients.FirstOrDefault(r => ((AppointmentWebSocketHandler)r)._userId == appointmentDTO.InitiatorId).Send(send);
            }

            /// <summary>
            /// If appointment accepted by initiator send notification to ad owner
            /// </summary>
            /// <param name="appointmentDTO">Appointment Model</param>
            public void AppointmentAcceptedByInitiator(AppointmentDTO appointmentDTO)
            {
                var appointmentNotification = _iAppointmentBus.UpdateAppointmentAcceptedByInitiator(appointmentDTO);
                var send = JsonConvert.SerializeObject(appointmentNotification);
                _clients.FirstOrDefault(r => ((AppointmentWebSocketHandler)r)._userId == appointmentDTO.AdvertismentOwnerId).Send(send);
            }

            /// <summary>
            /// Set appointments as seen an notify user
            /// </summary>
            /// <param name="appointmentDTO">Appointment Model</param>
            public void SetSeenAppointments(AppointmentDTO appointmentDTO)
            {
                _iAppointmentBus.SetAppointmentsAsSeen(appointmentDTO.UserWhoSeenId, appointmentDTO.SeenAppointmentsIds);
                appointmentDTO.AllSeen = true;
                var send = JsonConvert.SerializeObject(appointmentDTO);
                _clients.FirstOrDefault(r => ((AppointmentWebSocketHandler)r)._userId == appointmentDTO.UserWhoSeenId).Send(send);
            }
        }
    }
}
