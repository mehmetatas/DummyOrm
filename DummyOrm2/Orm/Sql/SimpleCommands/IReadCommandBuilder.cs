namespace DummyOrm2.Orm.Sql.SimpleCommands
{
    public interface IReadCommandBuilder : ISimpleCommandBuilder
    {
        SqlCommand BuildById<T>(object id);
    }
}