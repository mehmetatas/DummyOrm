using System;
using System.Linq.Expressions;

namespace DummyOrm.Meta
{
    public static class DbMetaExtenisons
    {
        public static TableMeta GetTable<T>(this DbMeta dbMeta) where T : class, new()
        {
            return dbMeta.GetTable(typeof(T));
        }

        public static DbMeta RegisterEntity<T>(this DbMeta dbMeta) where T : class, new()
        {
            return dbMeta.RegisterEntity(typeof(T));
        }

        public static DbMeta RegisterModel<T>(this DbMeta dbMeta) where T : class, new()
        {
            return dbMeta.RegisterModel(typeof(T));
        }

        public static ColumnMeta GetColumn<T, TProp>(this DbMeta dbMeta, Expression<Func<T, TProp>> propExp) where T : class, new()
        {
            return dbMeta.GetColumn(propExp.GetPropertyInfo());
        }

        public static AssociationMeta GetAssociation<T, TProp>(this DbMeta dbMeta, Expression<Func<T, TProp>> propExp) where T : class, new()
        {
            return dbMeta.GetAssociation(propExp.GetPropertyInfo());
        }
    }
}