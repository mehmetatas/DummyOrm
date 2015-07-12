using System;
using System.Linq.Expressions;

namespace DummyOrm2.Orm.Dynamix
{
    public static class PocoFactory
    {
        public static Func<T> CreateFactory<T>()
        {
            return Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
        }

        public static Func<object> CreateFactory(Type pocoType)
        {
            return (Func<object>)Expression.Lambda(typeof(Func<object>), Expression.New(pocoType)).Compile();
        }
    }
}