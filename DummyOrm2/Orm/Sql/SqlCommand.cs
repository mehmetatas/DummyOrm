using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DummyOrm2.Orm.Meta;
using DummyOrm2.Orm.Sql.Select;

namespace DummyOrm2.Orm.Sql
{
    public class SqlParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ColumnMeta ColumnMeta { get; set; }
    }

    public class SqlCommand
    {
        public string CommandText { get; set; }
        public IDictionary<string, SqlParameter> Parameters { get; set; }
    }

    public interface ISelectQueryBuilder<T> where T : class, new()
    {
        SelectQuery<T> Build();
    }

    public interface ISelectSqlCommandBuilder
    {
        SqlCommand Build<T>(SelectQuery<T> query) where T : class, new();
    }

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

            return new SqlCommand
            {
                CommandText = cmd.ToString().TrimEnd(),
                Parameters = param
            };
        }
    }
}
