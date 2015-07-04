namespace DummyOrm.Sql.QueryBuilders.Where
{
    public interface IWhereExpressionBuilder : IWhereExpressionVisitor
    {
        IWhereExpression Build();
    }
}