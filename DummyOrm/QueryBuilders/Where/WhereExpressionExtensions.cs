using System;
using System.Collections.Generic;
using System.Text;
using DummyOrm.QueryBuilders.Where.Expressions;

namespace DummyOrm.QueryBuilders.Where
{
    public static class WhereExpressionExtensions
    {
        public static void AppendTo(this IWhereExpression where, StringBuilder sql, IDictionary<string, object> parameters)
        {
            if (where == null)
            {
                return;
            }

            sql.Append(" WHERE ");

            var builder = new WhereSqlCommandBuilder();
            where.Accept(builder);

            var whereCmd = builder.Build();

            sql.Append(whereCmd.CommandText);

            foreach (var parameter in whereCmd.Parameters)
            {
                parameters.Add(parameter.Key, parameter.Value);
            }
        }

        public static void SetOperand(this BinaryExpression expression, IWhereExpression operand)
        {
            if (expression.Operand1 == null)
            {
                expression.Operand1 = operand;
            }
            else if (expression.Operand2 == null)
            {
                expression.Operand2 = operand;
            }
            else
            {
                throw new InvalidOperationException("All operands are alreadey set!");
            }
        }

        public static void SetOperand(this LogicalExpression expression, IWhereExpression operand)
        {
            if (expression.Operand1 == null)
            {
                expression.Operand1 = operand;
            }
            else if (expression.Operand2 == null)
            {
                expression.Operand2 = operand;
            }
            else
            {
                throw new InvalidOperationException("All operands are alreadey set!");
            }
        }
    }
}