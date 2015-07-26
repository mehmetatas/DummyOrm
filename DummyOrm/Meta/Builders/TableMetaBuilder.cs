using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DummyOrm.Meta.Builders
{
    class TableMetaBuilder<T> : ITableMetaBuilder<T> where T : class ,new()
    {
        private readonly IDbMeta _dbMeta;
        private readonly TableMeta _meta;
        private readonly IDbMetaBuilder _parentBuilder;

        public TableMetaBuilder(IDbMeta dbMeta, TableMeta meta, IDbMetaBuilder parentBuilder)
        {
            _dbMeta = dbMeta;
            _meta = meta;
            _parentBuilder = parentBuilder;
        }

        public ITableMetaBuilder<T> TableName(string tableName)
        {
            _meta.TableName = tableName;
            return this;
        }

        public IColumnMetaBuilder<T, TProp> Column<TProp>(Expression<Func<T, TProp>> propExp)
        {
            var columnMeta = _dbMeta.GetColumn(propExp);
            return new ColumnMetaBuilder<T, TProp>(columnMeta, this);
        }

        public ITableMetaBuilder<TOther> Table<TOther>() where TOther : class, new()
        {
            return _parentBuilder.Table<TOther>();
        }

        public IDbMetaBuilder OneToMany<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> listPropExp, Expression<Func<TMany, TOne>> foreignPropExp)
            where TOne : class, new()
            where TMany : class, new()
        {
            return _parentBuilder.OneToMany(listPropExp, foreignPropExp);
        }

        public IDbMetaBuilder ManyToMany<TParent, TAssoc>(Expression<Func<TParent, IList>> listPropExp)
            where TParent : class, new()
            where TAssoc : class, new()
        {
            return _parentBuilder.ManyToMany<TParent, TAssoc>(listPropExp);
        }
    }
}