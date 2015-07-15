using System.Data;
using DummyOrm2.Orm.Sql;

namespace DummyOrm2.Orm.Db
{
    public interface ICommandExecutor
    {
        int ExecuteNonQuery(SqlCommand sqlCmd);
        object ExecuteScalar(SqlCommand sqlCmd);
        IDataReader ExecuteReader(SqlCommand sqlCmd);
    }
}