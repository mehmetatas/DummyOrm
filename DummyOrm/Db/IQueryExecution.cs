using System.Collections.Generic;
using DummyOrm.Sql;

namespace DummyOrm.Db
{
    public interface IQueryExecution<T> where T : class, new()
    {
        T FirstOrDefault();

        IList<T> ToList();

        Page<T> Page(int page, int pageSize);

        Page<T> Top(int top);
    }
}