using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DummyOrm.Sql.Select;
using DummyOrm.Sql.Where;
using DummyOrm.Sql.Where.Expressions;
using DummyOrm.Sql.Where.ExpressionVisitors;

namespace DummyOrm.Sql
{
    public class SqlServerSelectCommandBuilderImpl : ISelectCommandBuilder
    {
        public readonly static SqlServerSelectCommandBuilderImpl Instance = new SqlServerSelectCommandBuilderImpl();

        public Command Build<T>(SelectQuery<T> query) where T : class, new()
        {
            var cmd = new StringBuilder();
            var param = new Dictionary<string, CommandParameter>();

            if (query.IsPaging)
            {
                cmd.AppendLine("WITH __DATA AS (");
            }

            cmd.Append("SELECT");

            if (query.IsTop)
            {
                cmd.AppendFormat(" TOP {0}", query.PageSize + 1);
            }

            cmd.AppendLine()
                .AppendLine(String.Join(",\n", query.SelectColumns.Values.Select(
                    c => String.Format("  {0}.{1} {2}", c.Table.Alias, c.Meta.ColumnName, c.Alias))))
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
                    Operator = Operator.And
                };

                foreach (var whereExpression in query.WhereExpressions.Skip(1))
                {
                    where.Operand2 = whereExpression;
                    where = new LogicalExpression
                    {
                        Operand1 = where,
                        Operator = Operator.And
                    };
                }

                var whereExp = where.Operand1;
                var builder = new WhereCommandBuilder();
                whereExp.Accept(builder);
                var whereCmd = builder.Build();

                cmd.AppendLine("WHERE")
                    .AppendLine(whereCmd.CommandText);
                foreach (var sqlParameter in whereCmd.Parameters)
                {
                    param.Add(sqlParameter.Key, sqlParameter.Value);
                }
            }

            if (query.IsPaging)
            {
                /*
                 with
                    __DATA as (SELECT...), 
                    __COUNT as (select count(0) as _ROWCOUNT from __DATA)
                select * from __COUNT, __DATA
                order by s.SalesOrderID
                offset 0 rows fetch next 10 rows only*/

                cmd.AppendLine(
                    "),\n__COUNT AS (SELECT COUNT(0) AS __ROWCOUNT FROM __DATA)\nSELECT * FROM __COUNT, __DATA")
                    .Append("ORDER BY ");

                if (query.OrderByColumns.Any())
                {
                    var comma = "";
                    foreach (var col in query.OrderByColumns)
                    {
                        cmd.AppendFormat("{0}__DATA.{1} {2}", comma, col.Column.Alias, col.Desc ? "DESC" : "ASC");
                        comma = ",";
                    }
                }
                else
                {
                    var fromCol = query.SelectColumns.Values.FirstOrDefault(c => c.Meta.Identity && c.Table == query.From);
                    
                    if (fromCol == null)
                    {
                        fromCol = query.SelectColumns.Values.FirstOrDefault(c => c.Meta.Identity);
                    }
                    
                    if (fromCol == null)
                    {
                        fromCol = query.SelectColumns.Values.FirstOrDefault();
                    }

                    cmd.AppendFormat("__DATA.{0}", fromCol.Alias);
                }

                cmd.AppendLine()
                    .AppendFormat("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY",
                        (query.Page - 1) * query.PageSize,
                        query.PageSize);
            }
            else
            {
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
            }


            return new Command
            {
                CommandText = cmd.ToString().TrimEnd(),
                Parameters = param
            };
        }
    }
}