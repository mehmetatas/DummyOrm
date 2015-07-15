using DummyOrm.Meta;

namespace DummyOrm.Sql.SimpleCommands
{
    public interface ISimpleCommandBuilder
    {
        void Register(TableMeta tableMeta);
        SqlCommand Build(object entity);
    }
}