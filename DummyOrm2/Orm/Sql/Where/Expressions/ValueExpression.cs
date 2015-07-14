namespace DummyOrm2.Orm.Sql.Where.Expressions
{
    public class ValueExpression : IWhereExpression
    {
        public object Value { get; set; }

        public void Accept(IWhereExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}