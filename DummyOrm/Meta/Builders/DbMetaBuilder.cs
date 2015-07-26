using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using DummyOrm.Providers;

namespace DummyOrm.Meta.Builders
{
    class DbMetaBuilder : IDbMetaBuilder
    {
        private readonly IDbMeta _meta;

        public DbMetaBuilder(IDbProvider provider)
        {
            _meta = provider.Meta;
        }

        public ITableMetaBuilder<T> Table<T>() where T : class, new()
        {
            var tableMeta = _meta.RegisterEntity<T>();
            return new TableMetaBuilder<T>(_meta, tableMeta, this);
        }

        public IDbMetaBuilder OneToMany<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> listPropExp, Expression<Func<TMany, TOne>> foreignPropExp)
            where TOne : class, new()
            where TMany : class, new()
        {
            throw new NotImplementedException();
        }

        public IDbMetaBuilder ManyToMany<TParent, TAssoc>(Expression<Func<TParent, IList>> listPropExp)
            where TParent : class, new()
            where TAssoc : class, new()
        {
            throw new NotImplementedException();
        }
    }
}