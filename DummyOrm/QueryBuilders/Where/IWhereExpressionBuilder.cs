namespace DummyOrm.QueryBuilders.Where
{
    public interface IWhereExpressionBuilder : IWhereExpressionVisitor
    {
        IWhereExpression Build();
    }
}