using System.Data;
using DummyOrm.Meta;
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

        public virtual ISelectCommandBuilder CreateSelectCommandBuilder(IDbMeta meta)
        {
            return new SqlServer2012SelectCommandBuilder(meta);
        }

        public virtual IWhereCommandBuilder CreateWhereCommandBuilder(IDbMeta meta)
        {
            return new SqlServer2012WhereCommandBuilder(this);
        }

        public virtual ICommandMetaBuilder CreateCommandMetaBuilder(IDbMeta meta)
        {
            return new SqlServer2012CommandMetaBuilder();
        }

        public IDeleteManyCommandBuilder CreateDeleteManyCommandBuilder(IDbMeta meta)
        {
            return new Sql2012DeleteWhereCommandBuilder(meta);
        }

        public abstract IDbConnection CreateConnection();
    }
}
