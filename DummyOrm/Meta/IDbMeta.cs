using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DummyOrm.Providers;
using DummyOrm.Sql.Command;

namespace DummyOrm.Meta
{
    public interface IDbMeta
    {
        IDbProvider DbProvider { get; }

        TableMeta RegisterEntity(Type type);

        TableMeta GetTable(Type type);

        ColumnMeta GetColumn(PropertyInfo propInf);

        IAssociationMeta GetAssociation(PropertyInfo prop);

        ManyToManyMeta ManyToMany<TParent, TAssoc>(Expression<Func<TParent, IList>> listPropExp)
            where TParent : class, new()
            where TAssoc : class, new();

        OneToManyMeta OneToMany<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> listPropExp,
            Expression<Func<TMany, TOne>> foreignPropExp)
            where TOne : class, new()
            where TMany : class, new();
    }
}