using System.Data;
using DummyOrm.Sql;

namespace DummyOrm.Db
{
    public interface ICommandExecutor
    {
        int ExecuteNonQuery(Command cmd);
        object ExecuteScalar(Command cmd);
        IDataReader ExecuteReader(Command cmd);
    }
}