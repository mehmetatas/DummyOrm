namespace DummyOrm2.Orm.Sql.Where.Expressions
{
    public class LikeExpression : IWhereExpression
    {
        public ColumnExpression Column { get; set; }
        public SqlOperator Operator { get; set; }
        public ValueExpression Value { get; set; }

        public void Accept(IWhereExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}