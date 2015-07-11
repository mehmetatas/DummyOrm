using System.Data;

namespace DummyOrm.Sql.QueryBuilders.Select
{
    public interface ISelectQueryExecutor
    {
        IDataReader Execute(SelectQueryMeta query);
    }
}