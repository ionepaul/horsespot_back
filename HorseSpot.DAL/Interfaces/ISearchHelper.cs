using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HorseSpot.DAL.Interfaces
{
    public interface ISearchHelper<T>
    {
        Expression<Func<T, bool>> GetSearchPredicate();
        PropertyInfo GetOrderProperty();
        bool IsAscendingSortOrder();
    }
}
