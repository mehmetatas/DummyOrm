using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace DummyOrm.Meta.Builders
{
    public interface IColumnMetaBuilder<T, TProp>
        where T : class, new()
    {
        IColumnMetaBuilder<T, TProp> ColumnName(string columnName);
        IColumnMetaBuilder<T, TProp> DbType(DbType dbType);
        IColumnMetaBuilder<T, TProp> Size(int size);
        IColumnMetaBuilder<T, TProp> Scale(byte scale);
        IColumnMetaBuilder<T, TProp> Precision(byte precision);
        IColumnMetaBuilder<T, TProp> AutoIncrement();
        IColumnMetaBuilder<T, TProp> Id();

        IColumnMetaBuilder<T, TOtherProp> Column<TOtherProp>(Expression<Func<T, TOtherProp>> propExp) where TOtherProp : class, new();
        ITableMetaBuilder<TOther> Table<TOther>() where TOther : class, new();
        IDbMetaBuilder OneToMany<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> listPropExp, Expression<Func<TMany, TOne>> foreignPropExp)
            where TOne : class, new()
            where TMany : class, new();
        IDbMetaBuilder ManyToMany<TParent, TAssoc>(Expression<Func<TParent, IList>> listPropExp)
            where TParent : class, new()
            where TAssoc : class, new();
    }
}