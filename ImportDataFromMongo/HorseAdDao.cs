using HorseSpot.DAL.Interfaces;
using HorseSpot.DAL.Models;
using MongoDB.Driver;
using System.Linq;
using ImportDataFromMongo;
using System.Collections.Generic;
using System;
using MongoDB.Driver.GridFS;
using MongoDB.Bson;
using HorseSpot.DAL.Entities;

namespace ImportDataFromMongo
{
    public class HorseAdDaoOld : MongoAbstractDao<HorseAd>
    {
        #region Constructor

        /// <summary>
        /// HorseAdDao Constructor
        /// </summary>
        /// <param name="dataContext">HorseSpot MongoDB Data Context</param>
        public HorseAdDaoOld(MongoDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion

        #region Public Methods

        public IEnumerable<HorseAdOld> GetAllForAdmin(int pageNumber)
        {
            var filter = Builders<HorseAdOld>.Filter.Empty;
            var sort = Builders<HorseAdOld>.Sort.Descending(e => e.DatePosted);
            return _context.HorseAds.Find(filter).ToList();
        }

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

        #endregion
    }
}