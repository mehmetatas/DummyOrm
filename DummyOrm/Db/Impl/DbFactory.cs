using System.Data.SqlClient;

namespace DummyOrm.Db.Impl
{
    public static class DbFactory
    {
        public static IDb Create(string connectionString)
        {
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return new DbImpl(conn);
        }
    }
}
