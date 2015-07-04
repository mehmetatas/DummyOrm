using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using DummyOrm.QueryBuilders.Select;

namespace DummyOrm.Repository
{
    public class RepositoryQuery<T> : IQuery<T>
    {
        private readonly IQueryBuilder<T> _queryBuilder; 
        private readonly ISelectQueryReader _reader;
        private SelectQuery _query;

        public RepositoryQuery(ISelectQueryReader reader, IQueryBuilder<T> queryBuilder)
        {
            _reader = reader;
            _queryBuilder = queryBuilder;
        }

        public IQuery<T> Join<TOther>(Expression<Func<TOther, T, bool>> joinOn)
        {
            _queryBuilder.Join<TOther>(joinOn);
            return this;
        }

        public IQuery<T> LeftJoin<TOther>(Expression<Func<TOther, T, bool>> joinOn)
        {
            _queryBuilder.LeftJoin<TOther>(joinOn);
            return this;
        }

        public IQuery<T> RightJoin<TOther>(Expression<Func<TOther, T, bool>> joinOn)
        {
            _queryBuilder.RightJoin<TOther>(joinOn);
            return this;
        }

        public IQuery<T> Join<T1, T2>(Expression<Func<T1, T2, bool>> joinOn)
        {
            _queryBuilder.Join(joinOn);
            return this;
        }

        public IQuery<T> LeftJoin<T1, T2>(Expression<Func<T1, T2, bool>> joinOn)
        {
            _queryBuilder.LeftJoin(joinOn);
            return this;
        }

        public IQuery<T> RightJoin<T1, T2>(Expression<Func<T1, T2, bool>> joinOn)
        {
            _queryBuilder.RightJoin(joinOn);
            return this;
        }

        public IQuery<T> Include<TOther>(params Expression<Func<TOther, object>>[] props)
        {
            _queryBuilder.Include(props);
            return this;
        }

        public IQuery<T> Exclude<TOther>(params Expression<Func<TOther, object>>[] props)
        {
            _queryBuilder.Exclude(props);
            return this;
        }

        public IQuery<T> OrderBy<TOther>(bool desc, params Expression<Func<TOther, object>>[] props)
        {
            _queryBuilder.OrderBy(desc, props);
            return this;
        }

        public IQuery<T> Where<TOther>(Expression<Func<TOther, bool>> filter)
        {
            _queryBuilder.Where(filter);
            return this;
        }

        public IQuery<T> Where<T1, T2>(Expression<Func<T1, T2, bool>> filter)
        {
            _queryBuilder.Where(filter);
            return this;
        }

        public IQuery<T> Where<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> filter)
        {
            _queryBuilder.Where(filter);
            return this;
        }

        public IQuery<T> Where<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> filter)
        {
            _queryBuilder.Where(filter);
            return this;
        }

        public IQuery<T> Where<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> filter)
        {
            _queryBuilder.Where(filter);
            return this;
        }

        public Page<T> Top(int top)
        {
            _queryBuilder.Top(top + 1);
            var items = Read().ToList();
            var totalCount = items.Count;
            
            return new Page<T>(1, top, totalCount, items.Take(top));
        }

        public Page<T> Page(int page, int pageSize)
        {
            _queryBuilder.Page(page, pageSize);
            var reader = ExecuteReader();
            return PageMapper.Map<T>(reader, DynamicPocoMapper.For<T>(), page, pageSize);
        }

        public T ReadFirst()
        {
            _queryBuilder.Top(1);
            return Read().FirstOrDefault();
        }

        public IEnumerable<T> Read()
        {
            var reader = ExecuteReader();
            return _query.IsSimpleMapping
                ? new PocoReader<T>(reader, DynamicPocoMapper.For<T>())
                : new PocoReader<T>(reader, new CustomPocoMapper(_query.OutputMappings));
        }

        public Page<Tuple<T1, T2>> Top<T1, T2>(int top)
        {
            _queryBuilder.Top(top + 1);
            var items = Read<T1, T2>().ToList();
            var totalCount = items.Count;

            return new Page<Tuple<T1, T2>>(1, top, totalCount, items.Take(top));
        }

        public Page<Tuple<T1, T2>> Page<T1, T2>(int page, int pageSize)
        {
            _queryBuilder.Page(page, pageSize);
            var reader = ExecuteReader();
            return PageMapper.Map<Tuple<T1, T2>>(reader, new TupleMapper<T1, T2>(_query.OutputMappings), page, pageSize);
        }

        public Tuple<T1, T2> ReadFirst<T1, T2>()
        {
            _queryBuilder.Top(1);
            return Read<T1, T2>().FirstOrDefault();
        }

        public IEnumerable<Tuple<T1, T2>> Read<T1, T2>()
        {
            var reader = ExecuteReader();
            return new PocoReader<Tuple<T1, T2>>(reader, new TupleMapper<T1, T2>(_query.OutputMappings));
        }

        private IDataReader ExecuteReader()
        {
            _query = _queryBuilder.Build();
            return _reader.ExecuteReader(_query);
        }
    }
}