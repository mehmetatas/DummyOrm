using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace DummyOrm.Meta.Builders
{
    class ColumnMetaBuilder<T, TProp> : IColumnMetaBuilder<T, TProp>
        where T : class, new()
    {
        private readonly ColumnMeta _meta;
        private readonly ITableMetaBuilder<T> _parentBuilder;

        public ColumnMetaBuilder(ColumnMeta meta, ITableMetaBuilder<T> parentBuilder)
        {
            _meta = meta;
            _parentBuilder = parentBuilder;
        }

        public IColumnMetaBuilder<T, TProp> ColumnName(string columnName)
        {
            _meta.ColumnName = columnName;
            return this;
        }

        public IColumnMetaBuilder<T, TProp> DbType(DbType dbType)
        {
            _meta.ParameterMeta.DbType = dbType;
            return this;
        }

        public IColumnMetaBuilder<T, TProp> Size(int size)
        {
            _meta.ParameterMeta.Size = size;
            return this;
        }

        public IColumnMetaBuilder<T, TProp> Scale(byte scale)
        {
            _meta.ParameterMeta.Scale = scale;
            return this;
        }

        public IColumnMetaBuilder<T, TProp> Precision(byte precision)
        {
            _meta.ParameterMeta.Precision = precision;
            return this;
        }

        public IColumnMetaBuilder<T, TProp> AutoIncrement()
        {
            _meta.AutoIncrement = true;
            return this;
        }

        public IColumnMetaBuilder<T, TProp> Id()
        {
            _meta.Identity = true;
            return this;
        }
        
        public IColumnMetaBuilder<T, TOtherProp> Column<TOtherProp>(Expression<Func<T, TOtherProp>> propExp) where TOtherProp : class, new()
        {
            return _parentBuilder.Column(propExp);
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