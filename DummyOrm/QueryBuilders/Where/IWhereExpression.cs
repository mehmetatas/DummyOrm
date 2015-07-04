namespace DummyOrm.QueryBuilders.Where
{
    public interface IWhereExpression
    {
        void Accept(IWhereExpressionVisitor visitor);
    }
}