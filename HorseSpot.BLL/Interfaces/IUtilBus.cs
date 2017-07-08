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
        void SaveImages(string adId, List<HttpPostedFile> files, string userId);
        Tuple<GridFSDownloadStream, string> GetImageById(string id);
        void DeleteImage(string adId, string imageId, string userId);
        Task SetUserProfilePicture(string path, string id);
        string GetUserPicturePath(string userId);
        Task EmailSendingBetweenUsers(EmailModelDTO emailModelDTO);
        void SetHorseAdProfilePicture(string adId, string imageId, string v);
        Task ReceiveEmailFromContactPage(ContactPageEmailModel emailModelDTO);
    }
}
