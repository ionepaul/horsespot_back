using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using HorseSpot.BLL.Interfaces;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Models.Enums;
using HorseSpot.Models.Models;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;

namespace HorseSpot.Api.Controllers
{
    public class AppointmentController : ApiController
    {
        private IAppointmentBus _iAppointmentBus;

        public AppointmentController(IAppointmentBus iAppointmentBus)
        {
            _iAppointmentBus = iAppointmentBus;
        }

        #region HttpGet

        [HttpGet]
        [Authorize]
        [Route("api/appointments/user/{userId}")]
        public UserAppointmentsViewModel GetUpcomingAppointmentsOwner([FromUri] string userId)
        {
            return _iAppointmentBus.GetUserAppointmentsViewModel(userId);
        }

        [HttpGet]
        [Authorize]
        [Route("api/appointments/unseen/{userId}")]
        public IEnumerable<AppointmentDTO> GetUnseenAppointmentsForUser([FromUri] string userId)
        {
            return _iAppointmentBus.GetUnseenAppointmentsForUser(userId);
        }

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

        #endregion

        #region HttpPost

        [HttpPost]
        [Authorize]
        [Route("api/appointments/cancel")]
        public void CancelAppointment([FromBody] CancelAppointmentModel cancelAppointmentModel)
        {
            _iAppointmentBus.CancelAppointment(cancelAppointmentModel);
        }

        #endregion

        #region WebSocket

        class AppointmentWebSocketHandler : WebSocketHandler
        {
            private static WebSocketCollection _clients = new WebSocketCollection();
            private IAppointmentBus _iAppointmentBus;
            private string _userId;

            public AppointmentWebSocketHandler(string userId, IAppointmentBus iAppointmentBus)
            {
                _iAppointmentBus = iAppointmentBus;
                _userId = userId;
            }

            public override void OnOpen()
            {
                _clients.Add(this);
            }

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

            public void AppoinmentCreated(AppointmentDTO appointmentDTO)
            {
                var appointmentNotification = _iAppointmentBus.MakeAppointment(appointmentDTO);
                var send = JsonConvert.SerializeObject(appointmentNotification);
                _clients.FirstOrDefault(r => ((AppointmentWebSocketHandler)r)._userId == appointmentDTO.AdvertismentOwnerId).Send(send);
            }

            public void AppointmentDateChangedByInitiator(AppointmentDTO appointmentDTO)
            {
                var appointmentNotification = _iAppointmentBus.UpdateAppointmentDateChangedByOwner(appointmentDTO);
                var send = JsonConvert.SerializeObject(appointmentNotification);
                _clients.FirstOrDefault(r => ((AppointmentWebSocketHandler)r)._userId == appointmentDTO.InitiatorId).Send(send);
            }

            public void AppointmentDateChangedByOwner(AppointmentDTO appointmentDTO)
            {
                var appointmentNotification = _iAppointmentBus.UpdateAppointmentDateChangedByInitiator(appointmentDTO);
                var send = JsonConvert.SerializeObject(appointmentNotification);
                _clients.FirstOrDefault(r => ((AppointmentWebSocketHandler)r)._userId == appointmentDTO.AdvertismentOwnerId).Send(send);
            }

            public void AppointmentAcceptedByOwner(AppointmentDTO appointmentDTO)
            {
                var appointmentNotification = _iAppointmentBus.UpdateAppointmentAcceptedByOwner(appointmentDTO);
                var send = JsonConvert.SerializeObject(appointmentNotification);
                _clients.FirstOrDefault(r => ((AppointmentWebSocketHandler)r)._userId == appointmentDTO.InitiatorId).Send(send);
            }

            public void AppointmentAcceptedByInitiator(AppointmentDTO appointmentDTO)
            {
                var appointmentNotification = _iAppointmentBus.UpdateAppointmentAcceptedByInitiator(appointmentDTO);
                var send = JsonConvert.SerializeObject(appointmentNotification);
                _clients.FirstOrDefault(r => ((AppointmentWebSocketHandler)r)._userId == appointmentDTO.AdvertismentOwnerId).Send(send);
            }

            public void SetSeenAppointments(AppointmentDTO appointmentDTO)
            {
                _iAppointmentBus.SetAppointmentsAsSeen(appointmentDTO.UserWhoSeenId, appointmentDTO.SeenAppointmentsIds);
                appointmentDTO.AllSeen = true;
                var send = JsonConvert.SerializeObject(appointmentDTO);
                _clients.FirstOrDefault(r => ((AppointmentWebSocketHandler)r)._userId == appointmentDTO.UserWhoSeenId).Send(send);
            }
        }

        #endregion
    }
}
