namespace DummyOrm.Sql.SimpleCommands
{
    interface IReadCommandBuilder : ISimpleCommandBuilder
    {
        Command BuildById<T>(object id);
    }
}