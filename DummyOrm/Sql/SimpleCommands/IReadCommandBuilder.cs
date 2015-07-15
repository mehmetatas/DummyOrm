namespace DummyOrm.Sql.SimpleCommands
{
    public interface IReadCommandBuilder : ISimpleCommandBuilder
    {
        SqlCommand BuildById<T>(object id);
    }
}