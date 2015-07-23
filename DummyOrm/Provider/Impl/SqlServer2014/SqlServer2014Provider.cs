using System.Data;
using DummyOrm.Sql;
using DummyOrm.Sql.Where;

namespace DummyOrm.Provider.Impl.SqlServer2014
{
    public abstract class SqlServer2014Provider : IDbProvider
    {
        public virtual char QuoteOpen
        {
            get { return '['; }
        }

        public virtual char QuoteClose
        {
            get { return ']'; }
        }

        public virtual char ParameterPrefix
        {
            get { return '@'; }
        }

        public virtual ISelectCommandBuilder CreateSelectCommandBuilder()
        {
            return new SqlServer2014SelectCommandBuilder();
        }

        public virtual IWhereCommandBuilder CreateWhereCommandBuilder()
        {
            return new SqlServer2014WhereCommandBuilder();
        }

        public virtual ICommandMetaBuilder CreateCommandMetaBuilder()
        {
            return new SqlServer2014CommandMetaBuilder();
        }

        public abstract IDbConnection CreateConnection();
    }
}
