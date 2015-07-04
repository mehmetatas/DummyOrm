namespace DummyOrm.QueryBuilders.Where.Expressions
{
    public class NotExpression : IWhereExpression
    {
        public IWhereExpression Operand { get; set; }

        public void Accept(IWhereExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}