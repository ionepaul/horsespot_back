using System;
using System.Linq.Expressions;

namespace HorseSpot.DAL.Interfaces
{
    public interface ISearchHelper<T>
    {
        Expression<Func<T, bool>> GetSearchPredicate();

        Expression<Func<T, object>> GetOrderPredicate();

        bool IsAscendingSortOrder();
    }
}
