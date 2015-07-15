using DummyOrm.Meta;

namespace DummyOrm.Sql.SimpleCommands
{
    public interface ISimpleCommandBuilder
    {
        void Register(TableMeta tableMeta);
        Command Build(object entity);
    }
}