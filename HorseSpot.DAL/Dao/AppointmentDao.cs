using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;

namespace HorseSpot.DAL.Dao
{
    public class AppointmentDao : AbstractDao<Appointment>, IAppointmentDao
    {
        #region Constructor

        public AppointmentDao(HorseSpotDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion

        #region Public Methods

        public IEnumerable<Appointment> GetUserAppointments(string userId)
        {
            return  _ctx.Appointments
                        .Where(x => x.AdvertismentOwnerId == userId || x.InitiatorId == userId)
                        .Where(x => !x.IsDatePassed)
                        .AsEnumerable();
        }

        public IEnumerable<Appointment> GetAppointmentsComingInTwoDays()
        {
            var appointments = from x in _ctx.Appointments
                               where (x.IsAccepted && !x.IsDatePassed) && 
                                     ((x.AppointmentDateTime.Month == DateTime.Now.Month && x.AppointmentDateTime.Day - DateTime.Now.Day == 2) || 
                                     (DbFunctions.AddDays(DateTime.Now, 2).Value.Month == x.AppointmentDateTime.Month && DbFunctions.AddDays(DateTime.Now, 2).Value.Day == x.AppointmentDateTime.Day))
                               select x;

            return appointments.AsEnumerable();
        }

        public IEnumerable<Appointment> GetDoneAppointments()
        {
            var appointments = from x in _ctx.Appointments
                               where (x.IsAccepted) &&
                                     ((x.AppointmentDateTime.Month == DateTime.Now.Month && DateTime.Now.Day - x.AppointmentDateTime.Day == 1) ||
                                     (DbFunctions.AddDays(DateTime.Now, -1).Value.Month == x.AppointmentDateTime.Month && DbFunctions.AddDays(DateTime.Now, -1).Value.Day == x.AppointmentDateTime.Day))
                               select x;

            return appointments.AsEnumerable();
        }

        public IEnumerable<Appointment> GetUnseenAppointments(string userId)
        {
            var appointments = from x in _ctx.Appointments
                               where ((!x.SeenByInitiator && x.InitiatorId == userId) || (!x.SeenByOwner && x.AdvertismentOwnerId == userId))
                               select x;

            return appointments.AsEnumerable();
        }

        public void UpdateAppointment(Appointment appointment)
        {
            _ctx.Entry(appointment).State = EntityState.Modified;
            _ctx.SaveChanges();
        }

        public IEnumerable<Appointment> GetAppointmentsByHorseAdvertismentId(int horseAdId)
        {
            var appointments = from app in _ctx.Appointments where app.AdvertismentId == horseAdId select app;

            return appointments.ToList();
        }

        #endregion
    }
}