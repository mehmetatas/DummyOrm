using System;
using System.Linq.Expressions;

namespace DummyOrm2.Orm.Meta
{
    public static class DbMetaExtenisons
    {
        public static TableMeta GetTable<T>(this DbMeta dbMeta)
        {
            return dbMeta.GetTable(typeof(T));
        }

        public static DbMeta Register<T>(this DbMeta dbMeta)
        {
            return dbMeta.Register(typeof(T));
        }

        public static ColumnMeta GetColumn<T, TProp>(this DbMeta dbMeta, Expression<Func<T, TProp>> propExp)
        {
            return dbMeta.GetColumn(propExp.GetPropertyInfo());
        }
    }
}