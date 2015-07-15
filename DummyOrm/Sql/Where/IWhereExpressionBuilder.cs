namespace DummyOrm.Sql.Where
{
    public interface IWhereExpressionBuilder : IWhereExpressionVisitor
    {
        IWhereExpression Build();
    }
}