using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DummyOrm.Repository
{
    public interface IQuery<T>
    {
        IQuery<T> Join<TProp>(Expression<Func<T, TProp>> refProp);

        IQuery<T> Where(Expression<Func<T, bool>> filter);
        
        T FirstOrDefault();

        IList<T> ToList();
    }
}