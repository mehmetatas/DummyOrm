using DummyOrm.Sql.QueryBuilders.Where.Expressions;
using DummyOrm.Sql.QueryBuilders.Where.ExpressionVisitors;

namespace DummyOrm.Sql.QueryBuilders.Where.ExpressionBuilders
{
    public class InExpressionBuilder : WhereExpressionBuilder
    {
        private readonly InExpression _expression;

        public InExpressionBuilder()
        {
            _expression = new InExpression();
        }

        public override void Visit(ValueExpression e)
        {
            _expression.Values = e;
        }

        public override void Visit(ColumnExpression e)
        {
            _expression.Column = e;
        }

        public override IWhereExpression Build()
        {
            return _expression;
        }
    }
}