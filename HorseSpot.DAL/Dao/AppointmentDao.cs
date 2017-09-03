using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace HorseSpot.DAL.Dao
{
    public class AppointmentDao : AbstractDao<Appointment>, IAppointmentDao
    {
        #region Constructor

        /// <summary>
        /// AppointmentDao Constructor
        /// </summary>
        /// <param name="dataContext">HorseSpot Relational Database COntext</param>
        public AppointmentDao(HorseSpotDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieve apppointments from database for a user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>List of appointments</returns>
        public IEnumerable<Appointment> GetUserAppointments(string userId)
        {
            return  _ctx.Appointments
                        .Where(x => x.AdvertismentOwnerId == userId || x.InitiatorId == userId)
                        .Where(x => !x.IsDatePassed)
                        .AsEnumerable();
        }

        /// <summary>
        /// Gets the appointments that are going to take place in two days
        /// </summary>
        /// <returns>List of appointments</returns>
        public IEnumerable<Appointment> GetAppointmentsComingInTwoDays()
        {
            var appointments = from x in _ctx.Appointments
                               where (x.IsAccepted && !x.IsDatePassed) && 
                                     ((x.AppointmentDateTime.Month == DateTime.Now.Month && x.AppointmentDateTime.Day - DateTime.Now.Day == 2) || 
                                     (DbFunctions.AddDays(DateTime.Now, 2).Value.Month == x.AppointmentDateTime.Month && DbFunctions.AddDays(DateTime.Now, 2).Value.Day == x.AppointmentDateTime.Day))
                               select x;

            return appointments.AsEnumerable();
        }

        /// <summary>
        /// Get the appointments that have took place one day before the current time
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Appointment> GetDoneAppointments()
        {
            var appointments = from x in _ctx.Appointments
                               where (x.IsAccepted) &&
                                     ((x.AppointmentDateTime.Month == DateTime.Now.Month && DateTime.Now.Day - x.AppointmentDateTime.Day == 1) ||
                                     (DbFunctions.AddDays(DateTime.Now, -1).Value.Month == x.AppointmentDateTime.Month && DbFunctions.AddDays(DateTime.Now, -1).Value.Day == x.AppointmentDateTime.Day))
                               select x;

            return appointments.AsEnumerable();
        }

        /// <summary>
        /// Gets the unseen appointments for a user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>List of appointments</returns>
        public IEnumerable<Appointment> GetUnseenAppointments(string userId)
        {
            var appointments = from x in _ctx.Appointments
                               where ((!x.SeenByInitiator && x.InitiatorId == userId) || (!x.SeenByOwner && x.AdvertismentOwnerId == userId))
                               select x;

            return appointments.AsEnumerable();
        }

        /// <summary>
        /// Update an appointment
        /// </summary>
        /// <param name="appointment">Appointment Model</param>
        public void UpdateAppointment(Appointment appointment)
        {
            _ctx.Entry(appointment).State = EntityState.Modified;
            _ctx.SaveChanges();
        }

        /// <summary>
        /// Gets the associated appointments for an advertisment
        /// </summary>
        /// <param name="horseAdId">Advertisment Id</param>
        /// <returns>List of appointments</returns>
        public IEnumerable<Appointment> GetAppointmentsByHorseAdvertismentId(string horseAdId)
        {
            var appointments = from app in _ctx.Appointments where app.AdvertismentId == horseAdId select app;

            return appointments.ToList();
        }

        #endregion
    }
}