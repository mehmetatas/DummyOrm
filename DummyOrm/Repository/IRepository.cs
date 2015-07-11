using System;

namespace DummyOrm.Repository
{
    public interface IRepository : IDisposable
    {
        void Insert(object entity);
        int Update(object entity);
        int Delete(object entity);
        IQuery<T> Select<T>();
    }
}