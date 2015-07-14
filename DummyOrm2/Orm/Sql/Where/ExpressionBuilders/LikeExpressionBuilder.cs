using DummyOrm2.Orm.Sql.Where.Expressions;
using DummyOrm2.Orm.Sql.Where.ExpressionVisitors;

namespace DummyOrm2.Orm.Sql.Where.ExpressionBuilders
{
    public class LikeExpressionBuilder : WhereExpressionBuilder
    {
        private readonly LikeExpression _expression;

        public LikeExpressionBuilder(SqlOperator oper)
        {
            _expression = new LikeExpression
            {
                Operator = oper
            };
        }

        public override void Visit(ValueExpression e)
        {
            _expression.Value = e;
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