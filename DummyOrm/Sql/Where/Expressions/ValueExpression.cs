using DummyOrm.Meta;

namespace DummyOrm.Sql.Where.Expressions
{
    public class ValueExpression : IWhereExpression
    {
        public object Value { get; set; }
        public ColumnMeta ColumnMeta { get; set; }

        public void Accept(IWhereExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}