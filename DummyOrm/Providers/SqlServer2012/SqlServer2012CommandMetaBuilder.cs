using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DummyOrm.Meta;
using DummyOrm.Sql.Command;

namespace DummyOrm.Providers.SqlServer2012
{
    public class SqlServer2012CommandMetaBuilder : ICommandMetaBuilder
    {
        public CommandMeta BuildInsertCommandMeta(TableMeta table)
        {
            var columns = table.Columns;

            var sql = new StringBuilder()
                .Append("INSERT INTO [")
                .Append(table.TableName)
                .Append("] (")
                .Append(String.Join(",", columns.Where(c => !c.AutoIncrement).Select(c => String.Format("[{0}]", c.ColumnName))))
                .Append(") VALUES (");

            var parameterMeta = new Dictionary<string, ColumnMeta>();

            var comma = "";
            foreach (var column in columns.Where(c => !c.AutoIncrement))
            {
                var paramName = String.Format("p{0}", parameterMeta.Count);

                sql.Append(comma)
                    .Append("@")
                    .Append(paramName);

                parameterMeta.Add(paramName, column);

                comma = ",";
            }

            sql.Append(")");

            if (!table.AssociationTable)
            {
                sql.Append("; SELECT SCOPE_IDENTITY();");
            }

            return new CommandMeta
            {
                CommandText = sql.ToString(),
                ParameterMeta = parameterMeta
            };
        }

        public CommandMeta BuildUpdateCommandMeta(TableMeta table)
        {
            var columns = table.Columns;

            var parameterMeta = new Dictionary<string, ColumnMeta>();

            var sql = new StringBuilder()
                .Append("UPDATE [")
                .Append(table.TableName)
                .Append("] SET ");

            var comma = "";
            foreach (var column in columns.Where(c => !c.Identity))
            {
                var paramName = String.Format("p{0}", parameterMeta.Count);

                sql.Append(comma)
                    .AppendFormat("[{0}]=@{1}", column.ColumnName, paramName);

                parameterMeta.Add(paramName, column);

                comma = ",";
            }

            BuildIdentityWhere(columns, parameterMeta, sql);

            return new CommandMeta
            {
                CommandText = sql.ToString(),
                ParameterMeta = parameterMeta
            };
        }

        public CommandMeta BuildDeleteCommandMeta(TableMeta table)
        {
            var columns = table.Columns;

            var parameterMeta = new Dictionary<string, ColumnMeta>();

            var sql = new StringBuilder()
                .Append("DELETE FROM [")
                .Append(table.TableName)
                .Append("]");

            BuildIdentityWhere(columns, parameterMeta, sql);

            return new CommandMeta
            {
                CommandText = sql.ToString(),
                ParameterMeta = parameterMeta
            };
        }

        public CommandMeta BuildGetByIdCommandMeta(TableMeta table)
        {
            var columns = table.Columns;

            var parameterMeta = new Dictionary<string, ColumnMeta>();

            var sql = new StringBuilder()
                .Append("SELECT ")
                .Append(String.Join(",", columns.Select(c => String.Format("[{0}]", c.ColumnName))))
                .Append(" FROM [")
                .Append(table.TableName)
                .Append("]");

            BuildIdentityWhere(columns, parameterMeta, sql);

            return new CommandMeta
            {
                CommandText = sql.ToString(),
                ParameterMeta = parameterMeta
            };
        }

        private static void BuildIdentityWhere(IEnumerable<ColumnMeta> columns, Dictionary<string, ColumnMeta> parameterMeta, StringBuilder sql)
        {
            var and = " WHERE ";
            foreach (var column in columns.Where(c => c.Identity))
            {
                var paramName = String.Format("wp{0}", parameterMeta.Count);

                sql.Append(and)
                    .AppendFormat("[{0}]=@{1}", column.ColumnName, paramName);

                parameterMeta.Add(paramName, column);

                and = " AND ";
            }
        }
    }
}
