using HorseSpot.DAL.Models;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Configuration;

namespace HorseSpot.DAL
{
    public class MongoDataContext
    {
        public IMongoDatabase _database;
        
        /// <summary>
        /// MongoDB Database Context Constructor
        /// </summary>     
        public MongoDataContext()
        {
            var client = new MongoClient(ConfigurationManager.AppSettings["HorseSpotMongoConnection"]);
            _database = client.GetDatabase(ConfigurationManager.AppSettings["HorseSpotMongoDb"]);
            ImagesBucket = new GridFSBucket(_database);
        }

        public GridFSBucket ImagesBucket { get; set; }

        public IMongoCollection<HorseAd> HorseAds => _database.GetCollection<HorseAd>("horseadvertisments");
    }
}
