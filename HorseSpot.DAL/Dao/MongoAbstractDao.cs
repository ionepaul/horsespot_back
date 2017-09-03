using HorseSpot.DAL.Interfaces;
using HorseSpot.DAL.Models;
using HorseSpot.Infrastructure.Constants;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorseSpot.DAL.Dao
{
    public class MongoAbstractDao<T> : IMongoDao<T> where T : AdvertismentBaseClass
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

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        public void Update(T entity)
        {
            _mongoCollection.ReplaceOne(r => r.Id == entity.Id, entity);
        }

        /// <summary>
        /// Set list of image ids for an advertisment
        /// </summary>
        /// <param name="id">Advertisment Id</param>
        /// <param name="imagesIds">List of uploaded image ids</param>
        public void SetImages(string id, List<string> imagesIds)
        {
            var filter = Builders<T>.Update.Set(x => x.ImageIds, imagesIds);
            var objectId = new ObjectId(id);

            _mongoCollection.UpdateOne(x => x.Id == objectId, filter);
        }

        /// <summary>
        /// Delete an entity, also all the associated images
        /// </summary>
        /// <param name="entity">entity to delete</param>
        public void Delete(T entity)
        {
            foreach (var imageId in entity.ImageIds)
            {
                _context.ImagesBucket.Delete(new ObjectId(imageId));
            }

            _mongoCollection.DeleteOne(e => e.Id == entity.Id);
        }
        
        /// <summary>
        /// Gets an advertisment by id
        /// </summary>
        /// <param name="stringId">Advertisment Id</param>
        /// <returns>Entity</returns>
        public T GetById(string stringId)
        {
            try
            {
                var id = new ObjectId(stringId);

                var filter = Builders<T>.Filter.Where(r => r.Id == id);

                return _mongoCollection.Find(filter).First();
            } 
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Validates an advertisment
        /// </summary>
        /// <param name="id">Advertisment Id</param>
        public void Validate(ObjectId id)
        {
            var filter = Builders<T>.Filter.Where(e => e.Id == id);
            var validate = Builders<T>.Update.Set(e => e.IsValidated, true);

            _mongoCollection.UpdateOne(filter, validate);
        }

        /// <summary>
        /// Update the wish list for a user 
        /// </summary>
        /// <param name="id">Advertisment Id</param>
        /// <param name="favoriteFor">List of users who have added it to wish list</param>
        public void UpdateFavoritesList(ObjectId id, IList<string> favoriteFor)
        {
            var filter = Builders<T>.Filter.Where(e => e.Id == id);
            var addFavorite = Builders<T>.Update.Set(e => e.FavoriteFor, favoriteFor);

            _mongoCollection.UpdateOne(filter, addFavorite);
        }

        /// <summary>
        /// Increase view of an advertismetn
        /// </summary>
        /// <param name="id">Advertisment Id</param>
        /// <param name="views">Number of views</param>
        /// <returns>Task</returns>
        public async Task IncreaseViews(ObjectId id, int views)
        {
            var filter = Builders<T>.Filter.Where(e => e.Id == id);
            var newViews = Builders<T>.Update.Set(e => e.Views, views);

            await _mongoCollection.UpdateOneAsync(filter, newViews);
        }

        #endregion
    }
}