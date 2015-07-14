using System.Data;
using DummyOrm2.Orm.Sql.Select;

namespace DummyOrm2.Orm.Db
{
    public interface IQueryExecuter
    {
        IDataReader Execute<T>(SelectQuery<T> query) where T : class, new();
    }
}