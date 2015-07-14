using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DummyOrm2.Orm.Meta;
using DummyOrm2.Orm.Sql;
using DummyOrm2.Orm.Sql.Select;

namespace DummyOrm2.Orm.Db
{
    public class QueryImpl<T> : IQuery<T>, ISelectQueryBuilder<T> where T : class, new()
    {
        private readonly IQueryExecuter _queryExecuter;
        private readonly SelectQuery<T> _query;

        public QueryImpl(IQueryExecuter queryExecuter)
        {
            _query = new SelectQuery<T>();
            _queryExecuter = queryExecuter;
        }

        public IQuery<T> Join<TProp>(Expression<Func<T, TProp>> refProp, Expression<Func<TProp, object>> include = null) where TProp : class ,new()
        {
            var propChain = refProp.GetPropertyChain();

            // Build join
            _query.Join(propChain);

            if (include == null)
            {
                // No columns specified Include all properties of joined type
                Include(refProp.GetMemberExpression());
            }
            else
            {
                if (include.Body is NewExpression)
                {
                    // Include multiple properties
                    Include(include.Body as NewExpression, propChain);
                }
                else
                {
                    // Include specified property only
                    Include(include.GetMemberExpression(), propChain);
                }
            }

            return this;
        }

        public IQuery<T> Include(Expression<Func<T, object>> propExpression)
        {
            if (propExpression.Body is NewExpression)
            {
                Include(propExpression.Body as NewExpression);
            }
            else
            {
                Include(propExpression.GetMemberExpression());
            }
            return this;
        }

        private void Include(NewExpression newExp, List<ColumnMeta> rootChain = null)
        {
            var includedProps = newExp.Arguments.Cast<MemberExpression>();
            foreach (var includeProp in includedProps)
            {
                Include(includeProp, rootChain);
            }
        }

        private void Include(MemberExpression memberExp, IEnumerable<ColumnMeta> rootChain = null)
        {
            var list = rootChain == null
                ? new List<ColumnMeta>()
                : new List<ColumnMeta>(rootChain);

            list.AddRange(memberExp.GetPropertyChain(false));
            _query.AddColumn(list);

            var colMeta = list.Last();
            if (colMeta.IsRefrence)
            {
                foreach (var columnMeta in colMeta.ReferencedTable.Columns)
                {
                    _query.AddColumn(new List<ColumnMeta>(list) { columnMeta });
                }
            }
        }

        public IWhereQuery<T> Where(Expression<Func<T, bool>> filter)
        {
            _query.Where(filter);
            return this;
        }

        public IOrderByQuery<T> OrderBy(Expression<Func<T, object>> props)
        {
            _query.OrderBy(props.GetPropertyChain(false), false);
            return this;
        }

        public IOrderByQuery<T> OrderByDesc(Expression<Func<T, object>> props)
        {
            _query.OrderBy(props.GetPropertyChain(false), true);
            return this;
        }

        public T FirstOrDefault()
        {
            var query = Build();

            using (var reader = _queryExecuter.Execute(query))
            {
                if (!reader.Read())
                {
                    return null;
                }

                var mapper = _query.CreateDeserializer();
                return mapper.Deserialize(reader) as T;
            }
        }

        public IList<T> ToList()
        {
            var query = Build();

            using (var reader = _queryExecuter.Execute(query))
            {
                var list = new List<T>();
                var mapper = _query.CreateDeserializer();

                while (reader.Read())
                {
                    list.Add((T)mapper.Deserialize(reader));
                }

                return list;
            }
        }

        public Page<T> Page(int page, int pageSize)
        {
            _query.SetPage(page, pageSize);

            var query = Build();

            using (var reader = _queryExecuter.Execute(query))
            {
                var list = new List<T>();
                var mapper = _query.CreateDeserializer();

                var totalCount = 0;

                while (reader.Read())
                {
                    if (totalCount == 0)
                    {
                        totalCount = Convert.ToInt32(reader["__ROWCOUNT"]);
                    }
                    list.Add((T)mapper.Deserialize(reader));
                }

                return new Page<T>(page, pageSize, totalCount, list);
            }
        }

        public Page<T> Top(int top)
        {
            _query.Top(top);

            var list = ToList();

            return new Page<T>(1, top, list.Count, list.Take(top));
        }

        public SelectQuery<T> Build()
        {
            if (!_query.SelectColumns.Any())
            {
                var fromTable = _query.From.Meta;
                foreach (var columnMeta in fromTable.Columns)
                {
                    _query.AddColumn(new[] { columnMeta });
                }
            }

            return _query;
        }
    }
}