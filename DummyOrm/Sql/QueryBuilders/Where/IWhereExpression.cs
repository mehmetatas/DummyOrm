namespace DummyOrm.Sql.QueryBuilders.Where
{
    public interface IWhereExpression
    {
        void Accept(IWhereExpressionVisitor visitor);
    }
}