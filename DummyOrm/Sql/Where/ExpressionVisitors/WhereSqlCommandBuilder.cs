using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DummyOrm.Sql.Where.Expressions;

namespace DummyOrm.Sql.Where.ExpressionVisitors
{
    /// <summary>
    /// Visits on IWhereExpressions
    /// Builds SqlCommand
    /// </summary>
    public class WhereSqlCommandBuilder : IWhereExpressionVisitor, IWhereSqlCommandBuilder
    {
        private readonly StringBuilder _sql = new StringBuilder();
        private readonly IDictionary<string, SqlParameter> _parameters = new Dictionary<string, SqlParameter>();

        public void Visit(LogicalExpression e)
        {
            _sql.Append("(");
            e.Operand1.Accept(this);
            _sql.Append(" ").Append(e.Operator.GetOperator()).Append(" ");
            e.Operand2.Accept(this);
            _sql.Append(")");
        }

        public void Visit(BinaryExpression e)
        {
            var left = e.Operand1;
            var right = e.Operand2;

            var isNull = false;

            if (e.Operand1 is NullExpression)
            {
                left = e.Operand2;
                right = e.Operand1;
                isNull = true;
            }
            else if (e.Operand2 is NullExpression)
            {
                isNull = true;
            }

            left.Accept(this);

            if (isNull)
            {
                _sql.Append(" IS");
                if (e.Operator == SqlOperator.NotEquals)
                {
                    _sql.Append(" NOT");
                }
            }
            else
            {
                _sql.Append(" ").Append(e.Operator.GetOperator()).Append(" ");
            }

            right.Accept(this);
        }

        public void Visit(ColumnExpression e)
        {
            _sql.AppendFormat("{0}.{1}", e.Column.Table.Alias, e.Column.Meta.ColumnName);
        }

        public void Visit(ValueExpression e)
        {
            if (e.Value == null)
            {
                throw new NotSupportedException("Null values should be handled by NullExpression");
            }

            var paramName = String.Format("wp{0}", _parameters.Count);
            _sql.AppendFormat("@{0}", paramName);
            _parameters.Add(paramName, new SqlParameter
            {
                Name = paramName,
                Value = e.Value,
                ColumnMeta = e.ColumnMeta
            });
        }

        public void Visit(NullExpression e)
        {
            _sql.Append(" NULL");
        }

        public void Visit(NotExpression e)
        {
            _sql.Append(" NOT (");
            e.Operand.Accept(this);
            _sql.Append(")");
        }

        public void Visit(LikeExpression e)
        {
            e.Column.Accept(this);

            switch (e.Operator)
            {
                case SqlOperator.LikeStartsWith:
                case SqlOperator.LikeContains:
                case SqlOperator.LikeEndsWith:
                    _sql.Append(" LIKE ");
                    break;
                case SqlOperator.NotLikeStartsWith:
                case SqlOperator.NotLikeContains:
                case SqlOperator.NotLikeEndsWith:
                    _sql.Append(" NOT LIKE ");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var value = (string)e.Value.Value;

            switch (e.Operator)
            {
                case SqlOperator.LikeStartsWith:
                case SqlOperator.NotLikeStartsWith:
                    value = value + "%";
                    break;
                case SqlOperator.LikeEndsWith:
                case SqlOperator.NotLikeEndsWith:
                    value = "%" + value;
                    break;
                case SqlOperator.LikeContains:
                case SqlOperator.NotLikeContains:
                    value = "%" + value + "%";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            e.Value.Value = value;

            Visit(e.Value);
        }

        public void Visit(InExpression e)
        {
            e.Column.Accept(this);

            var values = (IEnumerable)e.Values.Value;

            _sql.Append(" IN (");
            var comma = "";
            foreach (var val in values)
            {
                _sql.Append(comma);

                Visit(new ValueExpression
                {
                    Value = val, 
                    ColumnMeta = e.Values.ColumnMeta
                });

                comma = ",";
            }
            _sql.Append(")");
        }

        public SqlCommand Build()
        {
            return new SqlCommand
            {
                CommandText = _sql.ToString(),
                Parameters = _parameters
            };
        }
    }
}