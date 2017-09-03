using Quartz;
using Quartz.Impl;
using System;
using System.Configuration;

public class JobScheduler
{
    /// <summary>
    /// Schedule Jobs For Execution, it will be exectued each day
    /// </summary>
    public static void Start()
    {
        IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
        scheduler.Start();

        IJobDetail twoDaysBeforeAppointmentEmailJob = JobBuilder.Create<TwoDaysBeforeAppointmentEmailJob>().Build();
        IJobDetail appointmentsDoneEmailJob = JobBuilder.Create<AppointmentDoneEmailJob>().Build();

        var hour = Convert.ToInt32(ConfigurationManager.AppSettings["SchedulerHours"]);
        var minutes = Convert.ToInt32(ConfigurationManager.AppSettings["SchedulerMinutes"]);

        ITrigger twoDaysBeforeAppointmentTrigger = TriggerBuilder.Create().WithDailyTimeIntervalSchedule
                           (s => s.WithIntervalInHours(24).OnEveryDay()
                                 .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minutes))
                           ).Build();

        ITrigger appointmentsDoneTrigger = TriggerBuilder.Create().WithDailyTimeIntervalSchedule
                   (s => s.WithIntervalInHours(24).OnEveryDay()
                         .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minutes + 2))
                   ).Build();

        scheduler.ScheduleJob(twoDaysBeforeAppointmentEmailJob, twoDaysBeforeAppointmentTrigger);
        scheduler.ScheduleJob(appointmentsDoneEmailJob, appointmentsDoneTrigger);
    }
}