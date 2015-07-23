namespace DummyOrm.Sql
{
    public interface ISelectCommandBuilder
    {
        Command Build<T>(ISelectQuery<T> query) where T : class, new();
    }
}
