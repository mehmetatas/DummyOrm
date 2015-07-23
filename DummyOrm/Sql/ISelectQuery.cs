using System.Collections.Generic;
using DummyOrm.Sql.Select;
using DummyOrm.Sql.Where;

namespace DummyOrm.Sql
{
    public interface ISelectQuery<T> where T : class, new()
    {
        Table From { get; }
        IDictionary<string, Column> SelectColumns { get; }
        IDictionary<string, Join> Joins { get; }
        List<IWhereExpression> WhereExpressions { get; }
        List<OrderBy> OrderByColumns { get; }

        int Page { get; }
        int PageSize { get; }
    }

    public static class SelectQueryExtensions
    {
        public static bool IsPaging<T>(this ISelectQuery<T> query) where T : class, new()
        {
            return query.Page > 0 && query.PageSize > 0;
        }
        public static bool IsTop<T>(this ISelectQuery<T> query) where T : class, new()
        {
            return query.Page < 1 && query.PageSize > 0;
        }
    }
}