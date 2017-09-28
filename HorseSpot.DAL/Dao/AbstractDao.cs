using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using HorseSpot.DAL.Interfaces;

namespace HorseSpot.DAL.Dao
{
    public class AbstractDao<T> : IDisposable, IDao<T> where T : class
    {
        #region Local Variables

        protected HorseSpotDataContext _ctx;

        protected DbSet<T> _dbset;

        #endregion

        #region Constructor

        public AbstractDao(HorseSpotDataContext dataContext)
        {
            _ctx = dataContext;

            _dbset = dataContext.Set<T>();
        }

        #endregion

        #region Public Methods

        public virtual T GetById(int id)
        {
            return _dbset.Find(id);
        }

        public virtual void Add(T entity)
        {
            _dbset.Add(entity);
            _ctx.SaveChanges();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbset.AsEnumerable();
        }

        public virtual void SaveChanges()
        {
            _ctx.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            _dbset.Remove(entity);
            _ctx.SaveChanges();
        }

        public void Dispose()
        {
            _ctx.Dispose();
        }

        #endregion
    }
}
