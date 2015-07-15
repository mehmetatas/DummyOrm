using System.Data;
using DummyOrm.Sql;

namespace DummyOrm.Db
{
    public interface ICommandExecutor
    {
        int ExecuteNonQuery(SqlCommand sqlCmd);
        object ExecuteScalar(SqlCommand sqlCmd);
        IDataReader ExecuteReader(SqlCommand sqlCmd);
    }
}