namespace DummyOrm.Sql.Where.Expressions
{
    public class NullExpression : IWhereExpression
    {
        public object Value { get; set; }

        public void Accept(IWhereExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}