using System.Data;
using DummyOrm.Sql;
using DummyOrm.Sql.QueryBuilders.Select;

namespace DummyOrm.Repository
{
    public interface ISelectQueryReader
    {
        IDataReader ExecuteReader(SelectQuery query);
    }
}