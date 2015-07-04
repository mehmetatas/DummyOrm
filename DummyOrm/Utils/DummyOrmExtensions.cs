using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DummyOrm.Utils
{
    public static class DummyOrmExtensions
    {
        public static PropertyInfo GetPropertyInfo<T, TProp>(this Expression<Func<T, TProp>> propExpression)
        {
            MemberExpression memberExp;
            if (propExpression.Body is UnaryExpression)
            {
                memberExp = (MemberExpression)((UnaryExpression)propExpression.Body).Operand;
            }
            else
            {
                memberExp = (MemberExpression)propExpression.Body;
            }
            return memberExp.Member as PropertyInfo;
        }
    }
}