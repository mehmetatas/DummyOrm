using DummyOrm.Sql.QueryBuilders.Where.Expressions;
using DummyOrm.Sql.QueryBuilders.Where.ExpressionVisitors;

namespace DummyOrm.Sql.QueryBuilders.Where.ExpressionBuilders
{
    public class BinaryExpressionBuilder : WhereExpressionBuilder
    {
        private readonly BinaryExpression _expression;

        public BinaryExpressionBuilder(SqlOperator oper)
        {
            _expression = new BinaryExpression
            {
                Operator = oper
            };
        }

        public override void Visit(ColumnExpression e)
        {
            _expression.SetOperand(e);
        }

        public override void Visit(ValueExpression e)
        {
            _expression.SetOperand(e);
        }

        public override void Visit(NullExpression e)
        {
            _expression.SetOperand(e);
        }

        public override IWhereExpression Build()
        {
            return _expression;
        }
    }
}