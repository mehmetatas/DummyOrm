namespace DummyOrm.Sql.Command
{
    interface IReadCommandBuilder : ISimpleCommandBuilder
    {
        Command BuildById<T>(object id);
    }
}