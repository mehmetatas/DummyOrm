using System.Data;
using DummyOrm.Sql.Command;
using DummyOrm.Sql.Delete;
using DummyOrm.Sql.Select;
using DummyOrm.Sql.Where;

namespace DummyOrm.Providers.SqlServer2012
{
    public abstract class SqlServer2012Provider : IDbProvider
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
            return new SqlServer2012SelectCommandBuilder();
        }

        public virtual IWhereCommandBuilder CreateWhereCommandBuilder()
        {
            return new SqlServer2012WhereCommandBuilder();
        }

        public virtual ICommandMetaBuilder CreateCommandMetaBuilder()
        {
            return new SqlServer2012CommandMetaBuilder();
        }

        public IDeleteManyCommandBuilder CreateDeleteManyCommandBuilder()
        {
            return new Sql2012DeleteWhereCommandBuilder();
        }

        public abstract IDbConnection CreateConnection();
    }
}
