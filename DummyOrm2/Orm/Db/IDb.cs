using System;
using System.Collections;
using System.Linq.Expressions;

namespace DummyOrm2.Orm.Db
{
    public interface IDb
    {
        void Insert<T>(T entity) where T : class, new();

        void Update<T>(T entity) where T : class, new();

        void Delete<T>(T entity) where T : class, new();
        void Delete<T>(Expression<Func<T, bool>> filter) where T : class, new();

        T GetById<T>(object id) where T : class, new();
        IQuery<T> Select<T>() where T : class, new();

        IFillQuery<T> Select<T>(Expression<Func<T, IList>> list) where T : class, new();

        T CreateProxy<T>() where T : class, new();
    }
}