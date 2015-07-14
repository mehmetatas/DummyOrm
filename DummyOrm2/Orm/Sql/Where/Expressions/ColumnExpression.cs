
using DummyOrm2.Orm.Sql.Select;

namespace DummyOrm2.Orm.Sql.Where.Expressions
{
    public class ColumnExpression : IWhereExpression
    {
        public Column Column { get; set; }

        public void Accept(IWhereExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}