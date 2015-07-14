using System;
using DummyOrm2.Orm.Sql.Where.Expressions;

namespace DummyOrm2.Orm.Sql.Where.ExpressionVisitors
{
    /// <summary>
    /// Visits and Builds IWhereExpressions
    /// Acts like State in WhereExpressionVisitor
    /// </summary>
    public abstract class WhereExpressionBuilder : IWhereExpressionBuilder
    {
        public virtual void Visit(LogicalExpression e)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(ColumnExpression e)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(ValueExpression e)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(NullExpression e)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(BinaryExpression e)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(NotExpression e)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(LikeExpression e)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(InExpression e)
        {
            throw new NotImplementedException();
        }

        public abstract IWhereExpression Build();
    }
}