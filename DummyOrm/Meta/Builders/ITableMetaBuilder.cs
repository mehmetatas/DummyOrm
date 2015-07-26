using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DummyOrm.Meta.Builders
{
    public interface ITableMetaBuilder<T>
        where T : class, new()
    {
        ITableMetaBuilder<T> TableName(string tableName);
        IColumnMetaBuilder<T, TProp> Column<TProp>(Expression<Func<T, TProp>> propExp);

        ITableMetaBuilder<TOther> Table<TOther>() where TOther : class, new();
        IDbMetaBuilder OneToMany<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> listPropExp, Expression<Func<TMany, TOne>> foreignPropExp)
            where TOne : class, new()
            where TMany : class, new();
        IDbMetaBuilder ManyToMany<TParent, TAssoc>(Expression<Func<TParent, IList>> listPropExp)
            where TParent : class, new()
            where TAssoc : class, new();
    }
}