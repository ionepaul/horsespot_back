using HorseSpot.DAL.Models;
using MongoDB.Driver.GridFS;
using System;
using System.Web;

namespace HorseSpot.DAL.Interfaces
{
    public interface IUtilAdDao : IMongoDao<AdvertismentBaseClass>
    {
        Tuple<GridFSDownloadStream, string> GetImages(string imageId);
        void DeleteImage(string imageId);
        string UploadOneImage(HttpPostedFile image);
    }
}
