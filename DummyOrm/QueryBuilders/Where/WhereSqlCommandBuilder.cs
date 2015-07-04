using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DummyOrm.QueryBuilders.Where.Expressions;
using DummyOrm.Sql;

namespace DummyOrm.QueryBuilders.Where
{
    public class WhereSqlCommandBuilder : IWhereExpressionVisitor, IWhereSqlCommandBuilder
    {
        private readonly StringBuilder _sql = new StringBuilder();
        private readonly IDictionary<string, object> _parameters = new Dictionary<string, object>();

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
            _sql.Append(e.Column.Fullname);
        }

        public void Visit(ValueExpression e)
        {
            if (e.Value == null)
            {
                throw new NotSupportedException("Null values should be handled by NullExpression");
            }

            var paramName = String.Format("wp{0}", _parameters.Count);
            _sql.AppendFormat("@{0}", paramName);
            _parameters.Add(paramName, e.Value);
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
                    Value = val
                });
                comma = ",";
            }
            _sql.Append(")");
        }

        public SqlCommand Build()
        {
            return new SqlCommand(_sql.ToString(), _parameters);
        }
    }
}