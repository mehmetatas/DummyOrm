using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using DummyOrm.Sql.QueryBuilders.Select;

namespace DummyOrm.Repository
{
    /// <summary>
    /// Adaptor: ISelectQueryBuilder -> IQuery
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RepositoryQuery<T> : IQuery<T>
    {
        private readonly ISelectQueryBuilder<T> _queryBuilder;
        private readonly ISelectQueryExecutor _reader;
        private SelectQueryMeta _query;

        public RepositoryQuery(ISelectQueryExecutor reader, ISelectQueryBuilder<T> queryBuilder)
        {
            _reader = reader;
            _queryBuilder = queryBuilder;
        }

        public IQuery<T> Join<TProp>(Expression<Func<T, TProp>> refProp)
        {
            _queryBuilder.Join(refProp);
            return this;
        }

        public IQuery<T> Where(Expression<Func<T, bool>> filter)
        {
            _queryBuilder.Where(filter);
            return this;
        }

        public T FirstOrDefault()
        {
            using (var reader = ExecuteReader())
            {
                var pocoReader = new PocoReader<T>(reader, _query.Deserializer);
                return pocoReader.FirstOrDefault();
            }
        }

        public IList<T> ToList()
        {
            using (var reader = ExecuteReader())
            {
                var pocoReader = new PocoReader<T>(reader, _query.Deserializer);
                return pocoReader.ToList();
            }
        }

        private IDataReader ExecuteReader()
        {
            _query = _queryBuilder.Build();
            return _reader.Execute(_query);
        }
    }
}