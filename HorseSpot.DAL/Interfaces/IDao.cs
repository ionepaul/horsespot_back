using System.Collections.Generic;

namespace HorseSpot.DAL.Interfaces
{
    public interface IDao<T>
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void SaveChanges();

        void Delete(T entity);
    }
}
