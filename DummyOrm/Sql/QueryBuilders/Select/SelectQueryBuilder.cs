using DummyOrm.Meta;
using DummyOrm.Sql.QueryBuilders.Select.Graph;
using DummyOrm.Sql.QueryBuilders.Where;
using DummyOrm.Sql.QueryBuilders.Where.Expressions;
using DummyOrm.Sql.QueryBuilders.Where.ExpressionVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DummyOrm.Sql.QueryBuilders.Select
{
    public class SelectQueryBuilder<T> : ISelectQueryBuilder<T>
    {
        private readonly SelectGraph _selectGraph;
        private readonly List<IWhereExpression> _whereExpressions;

        public SelectQueryBuilder()
        {
            _selectGraph = new SelectGraph();
            _whereExpressions = new List<IWhereExpression>();

            var tableMeta = DbMeta.Instance.GetTable(typeof(T));
            _selectGraph.SetFromTable(tableMeta);
        }

        public ISelectQueryBuilder<T> Join<TProp>(Expression<Func<T, TProp>> prop)
        {
            _selectGraph.FromTable.Join(prop);
            return this;
        }

        public ISelectQueryBuilder<T> Where(Expression<Func<T, bool>> filter)
        {
            _whereExpressions.Add(WhereExpressionVisitor.Build(filter));
            return this;
        }

        public SelectQueryMeta Build()
        {
            var query = new SelectQueryMeta
            {
                SelectGraph = _selectGraph,
                Deserializer = _selectGraph.CreateDeserializer()
            };

            if (_whereExpressions.Any())
            {
                var where = new LogicalExpression
                {
                    Operand1 = _whereExpressions[0],
                    Operator = SqlOperator.And
                };

                foreach (var whereExpression in _whereExpressions.Skip(1))
                {
                    where.Operand2 = whereExpression;
                    where = new LogicalExpression
                    {
                        Operand1 = where,
                        Operator = SqlOperator.And
                    };
                }

                query.Where = where.Operand1;
            }

            return query;
        }
    }
}