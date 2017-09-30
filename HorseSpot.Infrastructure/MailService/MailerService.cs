using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using HorseSpot.Models.Models;
using RazorEngine;

namespace HorseSpot.Infrastructure.MailService
{
    public class MailerService : IMailerService
    {
        public async Task SendMail(EmailModel emailModel)
        {
            var message = new MailMessage();

            message.From = new MailAddress(emailModel.Sender);
            message.Subject = emailModel.EmailSubject;

            message.IsBodyHtml = true;
            
            var template = File.ReadAllText(HttpContext.Current.Server.MapPath(emailModel.EmailTemplatePath));
            message.Body = Razor.Parse(template, emailModel);
            
            message.To.Add(emailModel.Receiver);
            message.ReplyToList.Add(emailModel.Sender);

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = true;

                smtpClient.SendCompleted += (s, e) =>
                {
                    smtpClient.Dispose();
                    message.Dispose();
                };

                await smtpClient.SendMailAsync(message);
            }
        }

        /// <summary>
        /// Send synchronus emails for appointments
        /// </summary>
        /// <param name="emailModel">Email Model</param>
        public void SendMailAppointments(EmailModelAppointment emailModel)
        {
            var message = new MailMessage();

            message.From = new MailAddress(emailModel.Sender);
            message.Subject = emailModel.EmailSubject;

            message.IsBodyHtml = true;

            var template = "";

            if (emailModel.EmailTemplatePath.IndexOf("~") > -1)
            {
                template = File.ReadAllText(HttpContext.Current.Server.MapPath(emailModel.EmailTemplatePath));
            }
            else
            {
                template = File.ReadAllText(emailModel.EmailTemplatePath);
            }

            message.Body = Razor.Parse(template, emailModel);

            message.To.Add(emailModel.Receiver);
            message.ReplyToList.Add(emailModel.Sender);

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = true;

                smtpClient.SendCompleted += (s, e) =>
                {
                    smtpClient.Dispose();
                    message.Dispose();
                };

                smtpClient.Send(message);
            }
        }
    }
}
