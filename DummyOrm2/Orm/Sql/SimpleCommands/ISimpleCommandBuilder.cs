using DummyOrm2.Orm.Meta;

namespace DummyOrm2.Orm.Sql.SimpleCommands
{
    public interface ISimpleCommandBuilder
    {
        void Register(TableMeta tableMeta);
        SqlCommand Build(object entity);
    }
}