using DummyOrm.Meta;

namespace DummyOrm.Sql.SimpleCommands
{
    interface ISimpleCommandBuilder
    {
        void Register(TableMeta tableMeta);
        Command Build(object entity);
    }
}