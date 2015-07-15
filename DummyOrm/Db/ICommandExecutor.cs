using System.Data;
using DummyOrm.Sql;

namespace DummyOrm.Db
{
    public interface ICommandExecutor
    {
        int ExecuteNonQuery(Command sqlCmd);
        object ExecuteScalar(Command sqlCmd);
        IDataReader ExecuteReader(Command sqlCmd);
    }
}