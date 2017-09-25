using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportDataFromMongo
{
    public class MongoAbstractDao<T>
    {
        #region Local Variables

        protected MongoDataContext _context;
        protected IMongoCollection<T> _mongoCollection;

        #endregion

        #region Consturctor

        /// <summary>
        /// MongoAbstractDao Constructor
        /// </summary>
        /// <param name="context">HorseSpot MongoDB Data Context</param>
        public MongoAbstractDao(MongoDataContext context)
        {
            _context = context;
            _mongoCollection = context._database.GetCollection<T>(typeof(T).Name.ToLowerInvariant() + "vertisments");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public void Insert(T entity)
        {
            _mongoCollection.InsertOne(entity);
        }

        #endregion
    }
}