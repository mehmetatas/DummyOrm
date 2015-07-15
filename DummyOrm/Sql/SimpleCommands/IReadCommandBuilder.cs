namespace DummyOrm.Sql.SimpleCommands
{
    public interface IReadCommandBuilder : ISimpleCommandBuilder
    {
        Command BuildById<T>(object id);
    }
}