using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DummyOrm.Sql.QueryBuilders.Select.Graph
{
    public static class SelectGraphVisitor
    {
        public static string Visit(SelectGraph graph)
        {
            var sql = new StringBuilder("SELECT").AppendLine();

            if (!AppendSelectColumns(graph.FromTable, sql))
            {
                sql.AppendFormat("  {0}.*", graph.FromTable.Alias);
            }

            sql.AppendLine()
                .AppendFormat("FROM [{0}] {1}", graph.FromTable.Name, graph.FromTable.Alias);

            AppendJoins(graph.FromTable, sql);

            return sql.ToString();
        }

        private static void AppendJoins(TableNode tableNode, StringBuilder sql)
        {
            foreach (var join in tableNode.Joins)
            {
                sql.AppendLine()
                    .AppendFormat("  {0} JOIN [{1}] {2} ON {2}.{3} = {4}.{5}",
                        join.Type.ToString().ToUpperInvariant(),
                        join.ToTable.Name,
                        join.ToTable.Alias,
                        join.ToColumnName,
                        join.FromTable.Alias,
                        join.FromColumnName);

                AppendJoins(join.ToTable, sql);
            }
        }

        private static bool AppendSelectColumns(TableNode table, StringBuilder sql)
        {
            var columns = new List<ColumnNode>();

            AddSelectColumns(table, columns);

            if (!columns.Any())
            {
                return false;
            }

            sql.Append("  ")
                .Append(String.Join(",\n  ", columns.Select(c => String.Format("{0}.{1} {2}", c.TableAlias, c.Column.ColumnName, c.Alias))));

            return true;
        }

        private static void AddSelectColumns(TableNode table, List<ColumnNode> columns)
        {
            columns.AddRange(table.Columns);
            foreach (var join in table.Joins)
            {
                AddSelectColumns(join.ToTable, columns);
            }
        }
    }
}