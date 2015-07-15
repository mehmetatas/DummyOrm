using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DummyOrm2.Orm.Meta;

namespace DummyOrm2.Orm.Sql.SimpleCommands
{
    public class SqlCommandMeta
    {
        public string CommandText { get; private set; }
        public IDictionary<string, ColumnMeta> ParameterMeta { get; private set; }

        private SqlCommandMeta()
        {

        }

        public SqlCommand CreateCommand(object entity)
        {
            var parameters = new Dictionary<string, SqlParameter>();

            foreach (var kv in ParameterMeta)
            {
                var paramName = kv.Key;
                var colMeta = kv.Value;

                var value = colMeta.GetterSetter.Get(entity);

                if (colMeta.IsRefrence && value != null)
                {
                    value = colMeta.ReferencedTable.IdColumn.GetterSetter.Get(value);
                }

                parameters.Add(paramName, new SqlParameter
                {
                    Name = paramName,
                    Value = value ?? DBNull.Value,
                    ColumnMeta = colMeta
                });
            }

            return new SqlCommand
            {
                CommandText = CommandText,
                Parameters = parameters
            };
        }

        public static SqlCommandMeta CreateInsertCommandMeta(TableMeta table)
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

            return new SqlCommandMeta
            {
                CommandText = sql.ToString(),
                ParameterMeta = parameterMeta
            };
        }

        public static SqlCommandMeta CreateUpdateCommandMeta(TableMeta table)
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

            return new SqlCommandMeta
            {
                CommandText = sql.ToString(),
                ParameterMeta = parameterMeta
            };
        }

        public static SqlCommandMeta CreateDeleteCommandMeta(TableMeta table)
        {
            var columns = table.Columns;

            var parameterMeta = new Dictionary<string, ColumnMeta>();

            var sql = new StringBuilder()
                .Append("DELETE FROM [")
                .Append(table.TableName)
                .Append("]");

            BuildIdentityWhere(columns, parameterMeta, sql);

            return new SqlCommandMeta
            {
                CommandText = sql.ToString(),
                ParameterMeta = parameterMeta
            };
        }

        public static SqlCommandMeta CreateSelectCommandMeta(TableMeta table)
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

            return new SqlCommandMeta
            {
                CommandText = sql.ToString(),
                ParameterMeta = parameterMeta
            };
        }

        public static SqlCommandMeta CreateFillCommandMeta(TableMeta table)
        {
            var columns = table.Columns;

            var parameterMeta = new Dictionary<string, ColumnMeta>();

            var sql = new StringBuilder()
                .Append("SELECT pt.PostId, t.* FROM Tag t JOIN PostTag pt ON pt.TagId = t.Id WHERE pt.PostId IN (...)")
                .Append(String.Join(",", columns.Select(c => String.Format("[{0}]", c.ColumnName))))
                .Append(" FROM [")
                .Append(table.TableName)
                .Append("]");

            BuildIdentityWhere(columns, parameterMeta, sql);

            return new SqlCommandMeta
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