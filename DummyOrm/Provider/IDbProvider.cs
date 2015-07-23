using System.Data;
using DummyOrm.Sql;
using DummyOrm.Sql.Where;

namespace DummyOrm.Provider
{
    public interface IDbProvider
    {
        char QuoteOpen { get; }

        char QuoteClose { get; }

        char ParameterPrefix { get; }

        ISelectCommandBuilder GetSelectCommandBuilder();

        IWhereCommandBuilder GetWhereCommandBuilder();

        ICommandMetaBuilder GetCommandMetaBuilder();

        IDbConnection CreateConnection();
    }
}
