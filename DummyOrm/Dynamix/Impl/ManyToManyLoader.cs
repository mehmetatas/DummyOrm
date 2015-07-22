using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DummyOrm.Db;
using DummyOrm.Db.Impl;
using DummyOrm.Meta;
using DummyOrm.Sql.Select;
using DummyOrm.Sql.Where.Expressions;

namespace DummyOrm.Dynamix.Impl
{
    public class ManyToManyLoader<TAssoc> : IAssociationLoader
        where TAssoc : class, new()
    {
        private readonly ManyToManyMeta _meta;

        public ManyToManyLoader(ManyToManyMeta meta)
        {
            _meta = meta;
        }

        public void Load<T, TProp>(IList<T> parentEntities, ICommandExecutor cmdExec, Expression<Func<TProp, object>> includeProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            var parentId = _meta.ParentColumn.ReferencedTable.IdColumn;
            var parentIdGetter = parentId.GetterSetter;

            var parentIds = parentEntities.Select(parentIdGetter.Get);

            var query = new QueryImpl<TAssoc>(cmdExec);

            query.Join(new List<ColumnMeta> { _meta.ChildColumn }, includeProps);

            query.Where(new InExpression
            {
                Column = new ColumnExpression
                {
                    Column = new Column
                    {
                        Table = new Table { Meta = _meta.ParentColumn.Table, Alias = query.FromTableAlias },
                        Meta = _meta.ParentColumn
                    }
                },
                Values = new ValueExpression
                {
                    ColumnMeta = _meta.ParentColumn,
                    Value = parentIds
                }
            });

            var children = query.ToList();

            foreach (var assocObj in children)
            {                
                var parentObj = _meta.ParentColumn.GetterSetter.Get(assocObj);
                var parentIdValue = parentIdGetter.Get(parentObj);

                var parentEntity = parentEntities.First(pe => parentIdGetter.Get(pe).Equals(parentIdValue));

                var list = (IList)_meta.ListGetterSetter.Get(parentEntity);
                if (list == null)
                {
                    list = _meta.ListFactory();
                    _meta.ListGetterSetter.Set(parentEntity, list);
                }

                var childObj = _meta.ChildColumn.GetterSetter.Get(assocObj);

                list.Add(childObj);
            }
        }
    }
}