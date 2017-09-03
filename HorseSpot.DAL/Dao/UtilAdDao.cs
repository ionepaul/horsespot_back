using HorseSpot.DAL.Interfaces;
using HorseSpot.DAL.Models;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HorseSpot.DAL.Dao
{
    public class UtilAdDao : MongoAbstractDao<AdvertismentBaseClass>, IUtilAdDao
    {
        #region Constructor
        /// <summary>
        /// UtilDao Constructor
        /// </summary>
        /// <param name="dataContext">HorseSpot Mongo Database Context</param>
        public UtilAdDao(MongoDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Get an image from the database by id 
        /// </summary>
        /// <param name="imageId">Image Id</param>
        /// <returns>Tuple of GridFSDownladStream and contentType</returns>
        public Tuple<GridFSDownloadStream, string> GetImages(string imageId)
        {
            try
            {
                var stream = _context.ImagesBucket.OpenDownloadStream(new ObjectId(imageId));
                var img = stream.FileInfo;
                var contentType = img.Metadata["contentType"].AsString;

                return new Tuple<GridFSDownloadStream, string>(stream, contentType);
            }
            catch (GridFSFileNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Deletes an image from database by id
        /// </summary>
        /// <param name="id">Image Id</param>
        public void DeleteImage(string id)
        {
            _context.ImagesBucket.Delete(new ObjectId(id));
        }

        /// <summary>
        /// Upload one image to the database
        /// </summary>
        /// <param name="image">Http Posted Image</param>
        /// <returns>Image Id</returns>
        public string UploadOneImage(HttpPostedFile image)
        {
            GridFSUploadOptions options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument("contentType", image.ContentType)
            };

            var imageId = _context.ImagesBucket.UploadFromStream(image.FileName, image.InputStream, options);

            return imageId.ToString();
        }

        #endregion
    }
}
