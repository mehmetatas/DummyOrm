using DummyOrm.Sql.Where.ExpressionVisitors;

namespace DummyOrm.Providers.SqlServer2012
{
    public class SqlServer2012WhereCommandBuilder : WhereCommandBuilder
    {
        public SqlServer2012WhereCommandBuilder(IDbProvider provider) : base(provider)
        {
        }
    }
}
