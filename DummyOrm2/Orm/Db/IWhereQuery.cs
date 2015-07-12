using System;
using System.Linq.Expressions;

namespace DummyOrm2.Orm.Db
{
    public interface IWhereQuery<T> : IOrderByQuery<T>
    {
        IWhereQuery<T> Where(Expression<Func<T, bool>> filter);
    }
}