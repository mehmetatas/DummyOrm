using System;
using System.Linq.Expressions;

namespace DummyOrm.Sql.QueryBuilders.Select
{
    public interface _ISelectQueryBuilder<T>
    {
        SelectQuery Build();

        _ISelectQueryBuilder<T> Join<TOther>(Expression<Func<TOther, T, bool>> joinOn);
        _ISelectQueryBuilder<T> LeftJoin<TOther>(Expression<Func<TOther, T, bool>> joinOn);
        _ISelectQueryBuilder<T> RightJoin<TOther>(Expression<Func<TOther, T, bool>> joinOn);

        _ISelectQueryBuilder<T> Join<T1, T2>(Expression<Func<T1, T2, bool>> joinOn);
        _ISelectQueryBuilder<T> LeftJoin<T1, T2>(Expression<Func<T1, T2, bool>> joinOn);
        _ISelectQueryBuilder<T> RightJoin<T1, T2>(Expression<Func<T1, T2, bool>> joinOn);

        _ISelectQueryBuilder<T> Include<TOther>(params Expression<Func<TOther, object>>[] props);
        _ISelectQueryBuilder<T> Exclude<TOther>(params Expression<Func<TOther, object>>[] props);
        _ISelectQueryBuilder<T> OrderBy<TOther>(bool desc, params Expression<Func<TOther, object>>[] props);

        _ISelectQueryBuilder<T> Where<TOther>(Expression<Func<TOther, bool>> filter);
        _ISelectQueryBuilder<T> Where<T1, T2>(Expression<Func<T1, T2, bool>> filter);
        _ISelectQueryBuilder<T> Where<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> filter);
        _ISelectQueryBuilder<T> Where<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> filter);
        _ISelectQueryBuilder<T> Where<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> filter);

        _ISelectQueryBuilder<T> Top(int top);
        _ISelectQueryBuilder<T> Page(int page, int pageSize);

        //IQueryBuilder<T> GroupBy<TOther>(Expression<Func<TOther, object>> prop);
        //IQueryBuilder<T> IncludeCount<TOther>(Expression<Func<TOther, object>> prop, string alias);
        //IQueryBuilder<T> IncludeSum<TOther, TAggregate>(Expression<Func<TOther, object>> prop, string alias);
        //IQueryBuilder<T> IncludeMin<TOther, TAggregate>(Expression<Func<TOther, object>> prop, string alias);
        //IQueryBuilder<T> IncludeMax<TOther, TAggregate>(Expression<Func<TOther, object>> prop, string alias);
        //IQueryBuilder<T> IncludeAverage<TOther, TAggregate>(Expression<Func<TOther, object>> prop, string alias);

        //IQueryBuilder<T> Having<TAggregate>(string alias, Expression<Func<TAggregate, bool>> having);

        //IQueryBuilder<TOther> Union<TOther>();
        //IQueryBuilder<TOther> UnionAll<TOther>();
    }
    public static class QueryBuilderExtensions
    {
        public static _ISelectQueryBuilder<T> Include<T>(this _ISelectQueryBuilder<T> builder, params Expression<Func<T, object>>[] props)
        {
            return builder.Include(props);
        }

        public static _ISelectQueryBuilder<T> Exclude<T>(this _ISelectQueryBuilder<T> builder, params Expression<Func<T, object>>[] props)
        {
            return builder.Exclude(props);
        }

        public static _ISelectQueryBuilder<T> Where<T>(this _ISelectQueryBuilder<T> builder, Expression<Func<T, bool>> filter)
        {
            return builder.Where(filter);
        }

        public static _ISelectQueryBuilder<T> OrderBy<T>(this _ISelectQueryBuilder<T> builder, params Expression<Func<T, object>>[] props)
        {
            return builder.OrderBy(false, props);
        }

        public static _ISelectQueryBuilder<T> OrderByDesc<T>(this _ISelectQueryBuilder<T> builder, params Expression<Func<T, object>>[] props)
        {
            return builder.OrderBy(true, props);
        }
    }
}