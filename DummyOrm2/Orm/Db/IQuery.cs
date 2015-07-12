using System;
using System.Linq.Expressions;

namespace DummyOrm2.Orm.Db
{
    public interface IQuery<T> : IWhereQuery<T>
    {
        IQuery<T> Join<TProp>(Expression<Func<T, TProp>> refProp,
            Expression<Func<TProp, object>> include = null,
            Expression<Func<TProp, object>> exclude = null);

        IQuery<T> Include(Expression<Func<T, object>> props);

        IQuery<T> Exclude(Expression<Func<T, object>> props);
    }
}