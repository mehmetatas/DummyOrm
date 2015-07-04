using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DummyOrm.Repository
{
    public interface IQuery<T>
    {
        IQuery<T> Join<TOther>(Expression<Func<TOther, T, bool>> joinOn);
        IQuery<T> LeftJoin<TOther>(Expression<Func<TOther, T, bool>> joinOn);
        IQuery<T> RightJoin<TOther>(Expression<Func<TOther, T, bool>> joinOn);

        IQuery<T> Join<T1, T2>(Expression<Func<T1, T2, bool>> joinOn);
        IQuery<T> LeftJoin<T1, T2>(Expression<Func<T1, T2, bool>> joinOn);
        IQuery<T> RightJoin<T1, T2>(Expression<Func<T1, T2, bool>> joinOn);

        IQuery<T> Include<TOther>(params Expression<Func<TOther, object>>[] props);
        IQuery<T> Exclude<TOther>(params Expression<Func<TOther, object>>[] props);
        IQuery<T> OrderBy<TOther>(bool desc, params Expression<Func<TOther, object>>[] props);

        IQuery<T> Where<TOther>(Expression<Func<TOther, bool>> filter);
        IQuery<T> Where<T1, T2>(Expression<Func<T1, T2, bool>> filter);
        IQuery<T> Where<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> filter);
        IQuery<T> Where<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> filter);
        IQuery<T> Where<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> filter);
        
        T ReadFirst();
        IEnumerable<T> Read();
        Page<T> Top(int top);
        Page<T> Page(int page, int pageSize);

        Tuple<T1, T2> ReadFirst<T1, T2>();
        IEnumerable<Tuple<T1, T2>> Read<T1, T2>();
        Page<Tuple<T1, T2>> Top<T1, T2>(int top);
        Page<Tuple<T1, T2>> Page<T1, T2>(int page, int pageSize);
    }

    public static class QueryExtensions
    {
        public static IQuery<T> Include<T>(this IQuery<T> query, params Expression<Func<T, object>>[] props)
        {
            return query.Include(props);
        }

        public static IQuery<T> Exclude<T>(this IQuery<T> query, params Expression<Func<T, object>>[] props)
        {
            return query.Exclude(props);
        }

        public static IQuery<T> Where<T>(this IQuery<T> query, Expression<Func<T, bool>> filter)
        {
            return query.Where(filter);
        }

        public static IQuery<T> OrderBy<T>(this IQuery<T> query, params Expression<Func<T, object>>[] props)
        {
            return query.OrderBy(false, props);
        }

        public static IQuery<T> OrderByDesc<T>(this IQuery<T> query, params Expression<Func<T, object>>[] props)
        {
            return query.OrderBy(true, props);
        }
    }
}