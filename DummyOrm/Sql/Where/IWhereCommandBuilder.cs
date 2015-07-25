
namespace DummyOrm.Sql.Where
{
    public interface IWhereCommandBuilder : IWhereExpressionVisitor
    {
        Command.Command Build();
    }
}