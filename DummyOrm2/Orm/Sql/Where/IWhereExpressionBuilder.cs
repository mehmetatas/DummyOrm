namespace DummyOrm2.Orm.Sql.Where
{
    public interface IWhereExpressionBuilder : IWhereExpressionVisitor
    {
        IWhereExpression Build();
    }
}