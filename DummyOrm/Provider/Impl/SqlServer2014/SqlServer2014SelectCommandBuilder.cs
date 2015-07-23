using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DummyOrm.Meta;
using DummyOrm.Sql;
using DummyOrm.Sql.Where.Expressions;

namespace DummyOrm.Provider.Impl.SqlServer2014
{
    public class SqlServer2014SelectCommandBuilder : ISelectCommandBuilder
    {
        private readonly StringBuilder _cmd = new StringBuilder();
        private readonly Dictionary<string, CommandParameter> _param = new Dictionary<string, CommandParameter>();

        public Command Build<T>(ISelectQuery<T> query) where T : class, new()
        {
            if (query.IsPaging())
            {
                AppendPagingStart();
            }

            AppendSelect();

            if (query.IsTop())
            {
                AppendTop(query);
            }

            AppendColumnsAndFrom(query);

            AppendJoins(query);

            if (query.WhereExpressions.Any())
            {
                AppendWhere(query);
            }

            if (query.IsPaging())
            {
                AppendPaging(query);
            }
            else if (query.OrderByColumns.Any())
            {
                AppendOrderBy(query);
            }

            return new Command
            {
                CommandText = _cmd.ToString().TrimEnd(),
                Parameters = _param
            };
        }

        private void AppendPagingStart()
        {
            _cmd.AppendLine("WITH __DATA AS (");
        }

        private void AppendSelect()
        {
            _cmd.Append("SELECT");
        }

        private void AppendTop<T>(ISelectQuery<T> query) where T : class, new()
        {
            _cmd.AppendFormat(" TOP {0}", query.PageSize);
        }

        private void AppendColumnsAndFrom<T>(ISelectQuery<T> query) where T : class, new()
        {
            _cmd.AppendLine()
                .AppendLine(String.Join(",\n", query.SelectColumns.Values.Select(
                    c => String.Format("  {0}.{1} {2}", c.Table.Alias, c.Meta.ColumnName, c.Alias))))
                .AppendFormat("FROM [{0}] {1}", query.From.Meta.TableName, query.From.Alias)
                .AppendLine();
        }

        private void AppendOrderBy<T>(ISelectQuery<T> query) where T : class, new()
        {
            _cmd.Append("ORDER BY ");
            var comma = "";
            foreach (var col in query.OrderByColumns)
            {
                _cmd.AppendFormat("{0}{1}.{2} {3}", comma, col.Column.Table.Alias, col.Column.Meta.ColumnName,
                    col.Desc ? "DESC" : "ASC");
                comma = ",";
            }
        }

        private void AppendPaging<T>(ISelectQuery<T> query) where T : class, new()
        {
            /*
                 with
                    __DATA as (SELECT...), 
                    __COUNT as (select count(0) as _ROWCOUNT from __DATA)
                select * from __COUNT, __DATA
                order by s.SalesOrderID
                offset 0 rows fetch next 10 rows only*/

            _cmd.AppendLine(
                "),\n__COUNT AS (SELECT COUNT(0) AS __ROWCOUNT FROM __DATA)\nSELECT * FROM __COUNT, __DATA")
                .Append("ORDER BY ");

            if (query.OrderByColumns.Any())
            {
                var comma = "";
                foreach (var col in query.OrderByColumns)
                {
                    _cmd.AppendFormat("{0}__DATA.{1} {2}", comma, col.Column.Alias, col.Desc ? "DESC" : "ASC");
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

                _cmd.AppendFormat("__DATA.{0}", fromCol.Alias);
            }

            _cmd.AppendLine()
                .AppendFormat("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY",
                    (query.Page - 1) * query.PageSize,
                    query.PageSize);
        }

        private void AppendWhere<T>(ISelectQuery<T> query) where T : class, new()
        {
            var where = new LogicalExpression
            {
                Operand1 = query.WhereExpressions[0],
                Operator = Operator.And
            };

            foreach (var whereExpression in query.WhereExpressions.Skip(1))
            {
                @where.Operand2 = whereExpression;
                @where = new LogicalExpression
                {
                    Operand1 = @where,
                    Operator = Operator.And
                };
            }

            var whereExp = @where.Operand1;
            var builder = DbMeta.Instance.DbProvider.CreateWhereCommandBuilder();
            whereExp.Accept(builder);
            var whereCmd = builder.Build();

            _cmd.AppendLine("WHERE")
                .AppendLine(whereCmd.CommandText);
            foreach (var sqlParameter in whereCmd.Parameters)
            {
                _param.Add(sqlParameter.Key, sqlParameter.Value);
            }
        }

        private void AppendJoins<T>(ISelectQuery<T> query) where T : class, new()
        {
            foreach (var join in query.Joins.Select(j => j.Value))
            {
                _cmd.AppendFormat("  {0} JOIN [{1}] {2} ON {3}.{4} = {2}.{5}",
                    @join.Type.ToString().ToUpperInvariant(),
                    @join.RightColumn.Table.Meta.TableName,
                    @join.RightColumn.Table.Alias,
                    @join.LeftColumn.Table.Alias,
                    @join.LeftColumn.Meta.ColumnName,
                    @join.RightColumn.Meta.ColumnName)
                    .AppendLine();
            }
        }
    }
}