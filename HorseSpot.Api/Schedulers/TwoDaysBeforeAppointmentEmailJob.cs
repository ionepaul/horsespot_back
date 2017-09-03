using HorseSpot.BLL.Interfaces;
using Quartz;
using System.Web.Http;

public class TwoDaysBeforeAppointmentEmailJob : IJob
{
    private IAppointmentBus _iAppointmentBus;
     
    /// <summary>
    /// Send Email To Participants in an appointment that will take place in two days
    /// </summary>
    /// <param name="context">Job Exceution Context</param>
    public void Execute(IJobExecutionContext context)
    {
        _iAppointmentBus = (IAppointmentBus)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAppointmentBus));

        _iAppointmentBus.SendEmailForAppointmentsComingInTwoDays();
    }

}
