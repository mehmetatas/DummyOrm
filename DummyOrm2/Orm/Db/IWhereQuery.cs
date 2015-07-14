using System;
using System.Linq.Expressions;

namespace DummyOrm2.Orm.Db
{
    public interface IWhereQuery<T> : IOrderByQuery<T> where T : class, new()
    {
        IWhereQuery<T> Where(Expression<Func<T, bool>> filter);
    }
}