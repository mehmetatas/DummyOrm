using DummyOrm.Sql.Select;

namespace DummyOrm.Sql
{
    public interface ISelectCommandBuilder
    {
        Command Build<T>(SelectQuery<T> query) where T : class, new();
    }
}
