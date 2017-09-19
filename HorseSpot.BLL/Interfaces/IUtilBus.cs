using HorseSpot.Models.Models;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace HorseSpot.BLL.Interfaces
{
    public interface IUtilBus
    {
        string DeleteImage(int imageId, string userId);
        Task SetUserProfilePicture(string path, string id);
        Task EmailSendingBetweenUsers(EmailModelDTO emailModelDTO);
        void SetHorseAdProfilePicture(int imageId, string v);
        Task ReceiveEmailFromContactPage(ContactPageEmailModel emailModelDTO);
        void CheckFormat(string path);
    }
}
