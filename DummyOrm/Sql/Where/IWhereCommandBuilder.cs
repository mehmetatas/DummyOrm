using System;
using System.Linq.Expressions;
using DummyOrm.Sql.Where.ExpressionVisitors;

namespace DummyOrm.Sql.Where
{
    public interface IWhereCommandBuilder : IWhereExpressionVisitor
    {
        Command.Command Build();
    }

    public static class WhereCommandBuilderExtensions
    {
        public static Command.Command Build<T>(this IWhereCommandBuilder whereCmdBuilder, Expression<Func<T, bool>> filter, IWhereExpressionListener listener)
        {
            var whereExp = WhereExpressionVisitor.Build(filter, listener);
            whereExp.Accept(whereCmdBuilder);
            return whereCmdBuilder.Build();
        }
    }
}