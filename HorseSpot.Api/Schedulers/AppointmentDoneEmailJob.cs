using System.Web.Http;
using HorseSpot.BLL.Interfaces;
using Quartz;

public class AppointmentDoneEmailJob : IJob
{
    private IAppointmentBus _iAppointmentBus;

    public void Execute(IJobExecutionContext context)
    {
        _iAppointmentBus = (IAppointmentBus)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAppointmentBus));

        _iAppointmentBus.SendEmailForResolvedAppointments();
    }
}
