using System;

namespace DummyOrm.Sql.Where
{
    public enum SqlOperator
    {
        Not = 0,
        And = 1,
        Equals,
        GreaterThan,
        LessThan,
        In,
        LikeStartsWith,
        LikeEndsWith,
        LikeContains,
        // Nots
        Or = ~And,
        NotEquals = ~Equals,
        GreaterThanOrEquals = ~LessThan,
        LessThanOrEquals = ~GreaterThan,
        NotIn = ~In,
        NotLikeStartsWith = ~LikeStartsWith,
        NotLikeEndsWith = ~LikeEndsWith,
        NotLikeContains = ~LikeContains
    }

    public static class SqlOperatorExtensions
    {
        public static string GetOperator(this SqlOperator sqlOperator)
        {
            switch (sqlOperator)
            {
                case SqlOperator.Not:
                    return "NOT";
                case SqlOperator.And:
                    return "AND";
                case SqlOperator.Equals:
                    return "=";
                case SqlOperator.GreaterThan:
                    return ">";
                case SqlOperator.LessThan:
                    return "<";
                case SqlOperator.In:
                    return "IN";
                case SqlOperator.LikeStartsWith:
                case SqlOperator.LikeEndsWith:
                case SqlOperator.LikeContains:
                    return "LIKE";
                case SqlOperator.Or:
                    return "OR";
                case SqlOperator.NotEquals:
                    return "<>";
                case SqlOperator.GreaterThanOrEquals:
                    return ">=";
                case SqlOperator.LessThanOrEquals:
                    return "<=";
                case SqlOperator.NotIn:
                    return "NOT IN";
                case SqlOperator.NotLikeStartsWith:
                case SqlOperator.NotLikeEndsWith:
                case SqlOperator.NotLikeContains:
                    return "NOT LIKE";
                default:
                    throw new ArgumentOutOfRangeException("sqlOperator");
            }
        }
    }
}
