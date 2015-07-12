using System.Collections.Generic;
using DummyOrm2.Orm.Sql.Select;

namespace DummyOrm2.Orm.Db
{
    public interface IQueryExecution<T>
    {
        T FirstOrDefault();

        IList<T> ToList();

        Page<T> Page(int page, int pageSize);

        Page<T> Top(int top);
    }
}