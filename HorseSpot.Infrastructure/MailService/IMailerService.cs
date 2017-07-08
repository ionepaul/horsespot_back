using HorseSpot.Models.Models;
using System.Threading.Tasks;

namespace HorseSpot.Infrastructure.MailService
{
    public interface IMailerService
    {
        Task SendMail(EmailModel emailModel);

        void SendMailAppointments(EmailModelAppointment emailModel);
    }
}
