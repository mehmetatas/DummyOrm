namespace DummyOrm.Sql.Select
{
    public interface ISelectCommandBuilder
    {
        Command.Command Build(ISelectQuery query);
    }
}
