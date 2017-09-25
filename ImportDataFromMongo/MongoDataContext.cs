using System.Configuration;
using HorseSpot.DAL.Models;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace ImportDataFromMongo
{
    public class MongoDataContext
    {
        public IMongoDatabase _database;
  
        public MongoDataContext()
        {
            var client = new MongoClient(ConfigurationManager.AppSettings["HorseSpotMongoConnection"]);
            _database = client.GetDatabase(ConfigurationManager.AppSettings["HorseSpotMongoDb"]);
            ImagesBucket = new GridFSBucket(_database);
        }

        public GridFSBucket ImagesBucket { get; set; }

        public IMongoCollection<HorseAdOld> HorseAds => _database.GetCollection<HorseAdOld>("horseadvertisments");
    }
}
