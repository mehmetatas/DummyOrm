using System;
using System.Linq.Expressions;

namespace DummyOrm.Sql.QueryBuilders.Select
{
    public interface ISelectQueryBuilder<T>
    {
        SelectQueryMeta Build();

        ISelectQueryBuilder<T> Join<TProp>(Expression<Func<T, TProp>> prop);

        ISelectQueryBuilder<T> Where(Expression<Func<T, bool>> filter);
    }
}
