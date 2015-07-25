using DummyOrm.Meta;

namespace DummyOrm.Sql.Command
{
    interface ISimpleCommandBuilder
    {
        void Register(TableMeta tableMeta);
        Command Build(object entity);
    }
}