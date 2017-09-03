using HorseSpot.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseSpot.DAL.Dao
{
    public class AbstractDao<T> : IDisposable, IDao<T> where T : class
    {
        #region Local Variables

        protected HorseSpotDataContext _ctx;

        protected DbSet<T> _dbset;

        #endregion

        #region Constructor

        /// <summary>
        /// AbstractDao Constructor
        /// </summary>
        /// <param name="dataContext">HorseSpotDatabase Context</param>
        public AbstractDao(HorseSpotDataContext dataContext)
        {
            _ctx = dataContext;

            _dbset = dataContext.Set<T>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns></returns>
        public virtual T GetById(int id)
        {
            return _dbset.Find(id);
        }

        /// <summary>
        /// Adds entity to database
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Add(T entity)
        {
            _dbset.Add(entity);
            _ctx.SaveChanges();
        }

        /// <summary>
        /// Gets all entities from a database table
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            return _dbset.AsEnumerable();
        }

        /// <summary>
        /// Commits changes to database
        /// </summary>
        public virtual void SaveChanges()
        {
            _ctx.SaveChanges();
        }

        /// <summary>
        /// Delete entity from database
        /// </summary>
        /// <param name="entity">Enity</param>
        public virtual void Delete(T entity)
        {
            _dbset.Remove(entity);
            _ctx.SaveChanges();
        }

        /// <summary>
        /// Dispose class
        /// </summary>
        public void Dispose()
        {
            _ctx.Dispose();
        }

        #endregion
    }
}
