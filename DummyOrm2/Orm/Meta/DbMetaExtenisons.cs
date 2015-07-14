using System;
using System.Linq.Expressions;

namespace DummyOrm2.Orm.Meta
{
    public static class DbMetaExtenisons
    {
        public static TableMeta GetTable<T>(this DbMeta dbMeta) where T : class, new()
        {
            return dbMeta.GetTable(typeof(T));
        }

        public static DbMeta Register<T>(this DbMeta dbMeta) where T : class, new()
        {
            return dbMeta.Register(typeof(T));
        }

        public static ColumnMeta GetColumn<T, TProp>(this DbMeta dbMeta, Expression<Func<T, TProp>> propExp) where T : class, new()
        {
            return dbMeta.GetColumn(propExp.GetPropertyInfo());
        }
    }
}