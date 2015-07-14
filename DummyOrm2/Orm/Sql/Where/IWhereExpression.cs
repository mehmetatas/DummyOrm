namespace DummyOrm2.Orm.Sql.Where
{
    public interface IWhereExpression
    {
        void Accept(IWhereExpressionVisitor visitor);
    }
}