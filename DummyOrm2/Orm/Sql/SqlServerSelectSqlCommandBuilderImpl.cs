using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DummyOrm2.Orm.Sql.Select;
using DummyOrm2.Orm.Sql.Where;
using DummyOrm2.Orm.Sql.Where.Expressions;
using DummyOrm2.Orm.Sql.Where.ExpressionVisitors;

namespace DummyOrm2.Orm.Sql
{
    public class SqlServerSelectSqlCommandBuilderImpl : ISelectSqlCommandBuilder
    {
        public SqlCommand Build<T>(SelectQuery<T> query) where T : class, new()
        {
            var cmd = new StringBuilder();
            var param = new Dictionary<string, SqlParameter>();

            cmd.AppendLine("SELECT")
                .AppendLine(String.Join(",\n", query.SelectColumns.Values.Select(c => String.Format("  {0}.{1} {2}", c.Table.Alias, c.Meta.ColumnName, c.Alias))))
                .AppendFormat("FROM [{0}] {1}", query.From.Meta.TableName, query.From.Alias)
                .AppendLine();

            foreach (var join in query.Joins.Select(j => j.Value))
            {
                cmd.AppendFormat("  {0} JOIN [{1}] {2} ON {3}.{4} = {2}.{5}",
                    join.Type.ToString().ToUpperInvariant(),
                    join.RightColumn.Table.Meta.TableName,
                    join.RightColumn.Table.Alias,
                    join.LeftColumn.Table.Alias,
                    join.LeftColumn.Meta.ColumnName,
                    join.RightColumn.Meta.ColumnName)
                    .AppendLine();
            }

            if (query.WhereExpressions.Any())
            {
                var where = new LogicalExpression
                {
                    Operand1 = query.WhereExpressions[0],
                    Operator = SqlOperator.And
                };

                foreach (var whereExpression in query.WhereExpressions.Skip(1))
                {
                    where.Operand2 = whereExpression;
                    where = new LogicalExpression
                    {
                        Operand1 = where,
                        Operator = SqlOperator.And
                    };
                }

                var whereExp = where.Operand1;
                var builder = new WhereSqlCommandBuilder();
                whereExp.Accept(builder);
                var whereCmd = builder.Build();

                cmd.AppendLine("WHERE")
                    .AppendLine(whereCmd.CommandText);
                foreach (var sqlParameter in whereCmd.Parameters)
                {
                    param.Add(sqlParameter.Key, sqlParameter.Value);
                }
            }

            if (query.OrderByColumns.Any())
            {
                cmd.Append("ORDER BY ");
                var comma = "";
                foreach (var col in query.OrderByColumns)
                {
                    cmd.AppendFormat("{0}{1}.{2} {3}", comma, col.Column.Table.Alias, col.Column.Meta.ColumnName,
                        col.Desc ? "DESC" : "ASC");
                    comma = ",";
                }
            }

            return new SqlCommand
            {
                CommandText = cmd.ToString().TrimEnd(),
                Parameters = param
            };
        }
    }
}