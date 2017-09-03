using HorseSpot.BLL.Interfaces;
using Quartz;
using System.Web.Http;

public class AppointmentDoneEmailJob : IJob
{
    private IAppointmentBus _iAppointmentBus;

    /// <summary>
    /// Send emails to participants when an appointment has taken place
    /// </summary>
    /// <param name="context">Job Execution Context</param>
    public void Execute(IJobExecutionContext context)
    {
        _iAppointmentBus = (IAppointmentBus)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAppointmentBus));

        _iAppointmentBus.SendEmailForResolvedAppointments();
    }

}
