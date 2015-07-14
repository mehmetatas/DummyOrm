using DummyOrm2.Orm.Sql.Where.Expressions;
using DummyOrm2.Orm.Sql.Where.ExpressionVisitors;

namespace DummyOrm2.Orm.Sql.Where.ExpressionBuilders
{
    public class NotExpressionBuilder : WhereExpressionBuilder
    {
        private readonly NotExpression _expression;

        public NotExpressionBuilder()
        {
            _expression = new NotExpression();
        }

        public override void Visit(BinaryExpression e)
        {
            _expression.Operand = e;
        }

        public override void Visit(InExpression e)
        {
            _expression.Operand = e;
        }

        public override void Visit(LikeExpression e)
        {
            _expression.Operand = e;
        }

        public override void Visit(LogicalExpression e)
        {
            _expression.Operand = e;
        }

        public override void Visit(NotExpression e)
        {
            _expression.Operand = e;
        }

        public override IWhereExpression Build()
        {
            return _expression;
        }
    }
}