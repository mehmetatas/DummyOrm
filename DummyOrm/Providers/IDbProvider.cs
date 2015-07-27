using System.Data;
using DummyOrm.Meta;
using DummyOrm.Sql.Command;
using DummyOrm.Sql.Delete;
using DummyOrm.Sql.Select;
using DummyOrm.Sql.Where;

namespace DummyOrm.Providers
{
    public interface IDbProvider
    {
        char QuoteOpen { get; }

        char QuoteClose { get; }

        char ParameterPrefix { get; }

        ISelectCommandBuilder CreateSelectCommandBuilder(IDbMeta meta);

        IWhereCommandBuilder CreateWhereCommandBuilder(IDbMeta meta);

        ICommandMetaBuilder CreateCommandMetaBuilder(IDbMeta meta);

        IDeleteManyCommandBuilder CreateDeleteManyCommandBuilder(IDbMeta meta);

        IDbConnection CreateConnection();
    }
}
