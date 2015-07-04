using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DummyOrm.Meta;

namespace DummyOrm.Sql.QueryBuilders
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
            var parameters = ParameterMeta.ToDictionary(kv => kv.Key, kv => new SqlCommandParameter
            {
                Name = kv.Key,
                Value = kv.Value.GetValue(entity) ?? DBNull.Value,
                Type = kv.Value.GetParamType()
            });
            return new SqlCommand(CommandText, parameters);
        }

        public static SqlCommandMeta CreateInsertCommandMeta(Type type)
        {
            var table = DbMeta.Instance.GetTable(type);
            var columns = table.Columns;

            var sql = new StringBuilder()
                .Append("INSERT INTO ")
                .Append(table.QuotedName)
                .Append(" (")
                .Append(String.Join(",", columns.Where(c => !c.AutoIncrement).Select(c => c.QuotedName)))
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

        public static SqlCommandMeta CreateUpdateCommandMeta(Type type)
        {
            var table = DbMeta.Instance.GetTable(type);
            var columns = table.Columns;

            var parameterMeta = new Dictionary<string, ColumnMeta>();

            var sql = new StringBuilder()
                .Append("UPDATE ")
                .Append(table.QuotedName)
                .Append(" SET ");

            var comma = "";
            foreach (var column in columns.Where(c => !c.Identity))
            {
                var paramName = String.Format("p{0}", parameterMeta.Count);

                sql.Append(comma)
                    .AppendFormat("{0}=@{1}", column.QuotedName, paramName);

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

        public static SqlCommandMeta CreateDeleteCommandMeta(Type type)
        {
            var table = DbMeta.Instance.GetTable(type);
            var columns = table.Columns;

            var parameterMeta = new Dictionary<string, ColumnMeta>();

            var sql = new StringBuilder()
                .Append("DELETE FROM ")
                .Append(table.QuotedName);

            BuildIdentityWhere(columns, parameterMeta, sql);

            return new SqlCommandMeta
            {
                CommandText = sql.ToString(),
                ParameterMeta = parameterMeta
            };
        }

        public static SqlCommandMeta CreateSelectCommandMeta(Type type)
        {
            var table = DbMeta.Instance.GetTable(type);
            var columns = table.Columns;

            var parameterMeta = new Dictionary<string, ColumnMeta>();

            var sql = new StringBuilder()
                .Append("SELECT ")
                .Append(String.Join(",", columns.Select(c => String.Format("{0} {1}", c.Column.Fullname, c.Column.Alias))))
                .Append(" FROM ")
                .Append(table.QuotedName);

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
                var paramName = String.Format("p{0}", parameterMeta.Count);

                sql.Append(and)
                    .AppendFormat("{0}=@{1}", column.QuotedName, paramName);

                parameterMeta.Add(paramName, column);

                and = " AND ";
            }
        }
    }
}