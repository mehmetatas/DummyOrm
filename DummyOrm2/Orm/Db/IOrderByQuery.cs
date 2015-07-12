using System;
using System.Linq.Expressions;

namespace DummyOrm2.Orm.Db
{
    public interface IOrderByQuery<T> : IQueryExecution<T>
    {
        IOrderByQuery<T> OrderBy(Expression<Func<T, object>> props);

        IOrderByQuery<T> OrderByDesc(Expression<Func<T, object>> props);
    }
}