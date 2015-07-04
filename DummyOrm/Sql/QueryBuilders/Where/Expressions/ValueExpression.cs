using System;

namespace DummyOrm.Sql.QueryBuilders.Where.Expressions
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