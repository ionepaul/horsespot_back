using HorseSpot.Infrastructure.MailService;
using HorseSpot.Models.Models;
using System.Configuration;

namespace HorseSpot.BLL.Converters
{
    public static class EmailSendingConverter
    {
        /// <summary>
        /// Creates an Email Model from an EmailModelDTO that is received from email sending between users
        /// </summary>
        /// <param name="emailModelDTO">EmailModelDTO Model</param>
        /// <returns>EmailModel</returns>
        public static EmailModel FromEmailModelDTOTOEmailModel(EmailModelDTO emailModelDTO)
        {
            return new EmailModel
            {
                Sender = emailModelDTO.Sender,
                Receiver = emailModelDTO.Receiver,
                EmailMessage = emailModelDTO.Message,
                EmailSubject = EmailSubjects.EmailFromApplication,
                EmailTemplatePath = EmailTemplatesPath.EmailFromApplicationTemplate,
                SenderName = emailModelDTO.SenderName,
                ReceiverFirstName = emailModelDTO.ReceiverFirstName,
                HorseAdTitle = emailModelDTO.HorseAdTitle
            };
        }

        /// <summary>
        /// Creates an EmailModel from ContactPageEmailModel
        /// </summary>
        /// <param name="contactPageEmailModel">ConactPageEmailModel</param>
        /// <returns>EmaiModel</returns>
        public static EmailModel FromContactPageEmailModelTOEmailModel(ContactPageEmailModel contactPageEmailModel)
        {
            return new EmailModel
            {
                Sender = contactPageEmailModel.Sender,
                Receiver = ConfigurationManager.AppSettings["AdminEmail"],
                EmailMessage = contactPageEmailModel.Message,
                EmailSubject = EmailSubjects.ContactPageEmailSubject,
                EmailTemplatePath = EmailTemplatesPath.ContactPageEmailTemplate,
                SenderName = contactPageEmailModel.SenderName
            };
        }
    }
}
