namespace DummyOrm.Sql.Where
{
    public interface IWhereExpression
    {
        void Accept(IWhereExpressionVisitor visitor);
    }
}