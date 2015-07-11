using System.Collections.Generic;
using System.Text;
using DummyOrm.Execution;
using DummyOrm.Sql.QueryBuilders.Select.Graph;
using DummyOrm.Sql.QueryBuilders.Where;
using DummyOrm.Sql.QueryBuilders.Where.ExpressionVisitors;

namespace DummyOrm.Sql.QueryBuilders.Select
{
    public class SelectQueryMeta
    {
        public SelectGraph SelectGraph { get; set; }
        public IWhereExpression Where { get; set; }
        public IPocoDeserializer Deserializer { get; set; }

        public SqlCommand ToSqlCommand()
        {
            var sqlSelectJoin = SelectGraphVisitor.Visit(SelectGraph);

            if (Where == null)
            {
                return new SqlCommand(sqlSelectJoin, new Dictionary<string, SqlCommandParameter>());
            }

            var whereBuilder = new WhereSqlCommandBuilder(SelectGraph);

            Where.Accept(whereBuilder);

            var wherePart = whereBuilder.Build();

            var sql = new StringBuilder(sqlSelectJoin).Append(" WHERE ").Append(wherePart.CommandText);
            
            return new SqlCommand(sql.ToString(), wherePart.Parameters);
        }
    }
}