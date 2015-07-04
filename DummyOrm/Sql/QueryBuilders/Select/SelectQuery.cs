using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DummyOrm.Meta;
using DummyOrm.Sql.QueryBuilders.Where;

namespace DummyOrm.Sql.QueryBuilders.Select
{
    public class SelectQuery
    {
        public string Table { get; set; }
        public IEnumerable<Column> SelectColumns { get; set; }
        public IWhereExpression Where { get; set; }
        public IEnumerable<OrderBy> OrderByColumns { get; set; }
        public IEnumerable<Join> Joins { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public IDictionary<string, ColumnMeta> OutputMappings { get; set; }

        public bool IsSimpleMapping { get; set; }

        public bool IsTopQuery
        {
            get { return PageIndex < 1 && PageSize > 0; }
        }

        public bool IsPagingQuery
        {
            get { return PageIndex > 0 && PageSize > 0; }
        }

        public SqlCommand ToSqlCommand()
        {
            var parameters = new Dictionary<string, SqlCommandParameter>();

            var sql = new StringBuilder("SELECT ");

            if (IsTopQuery)
            {
                sql.Append("TOP ").Append(PageSize).Append(" ");
            }

            sql.Append(String.Join(",", SelectColumns
                .Select(c => String.Format("{0} {1}", c.Fullname, c.Alias))));

            if (IsPagingQuery)
            {
                sql.Append(", COUNT(*) OVER() AS __ROWCOUNT");
            }

            sql.Append(" FROM ")
                .Append(Table);

            if (Joins != null)
            {
                foreach (var join in Joins)
                {
                    if (join.Type == JoinType.Left)
                    {
                        sql.Append(" LEFT");
                    }
                    else if (join.Type == JoinType.Right)
                    {
                        sql.Append(" RIGHT");
                    }

                    sql.Append(" JOIN [")
                        .Append(join.Column1.Table)
                        .Append("] ON ")
                        .Append(join.Column1.Fullname)
                        .Append(" = ")
                        .Append(join.Column2.Fullname);
                }
            }

            Where.AppendTo(sql, parameters);

            if (OrderByColumns != null && OrderByColumns.Any())
            {
                sql.Append(" ORDER BY ")
                    .Append(String.Join(",", OrderByColumns
                        .Select(c => String.Format("{0} {1}", c.Column.Fullname, c.Desc ? "DESC" : "ASC"))));
            }

            if (IsPagingQuery)
            {
                sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", (PageIndex - 1) * PageSize, PageSize);
            }

            return new SqlCommand(sql.ToString(), parameters);
        }
    }
}
