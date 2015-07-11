using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DummyOrm.Meta;

namespace DummyOrm.Execution
{
    public static class PropertyMapperFactory
    {
        public static PropertyMapper Create<T, TProp>(Expression<Func<T, TProp>> propExp)
        {
            var propChain = CreateColumnMetaChain(propExp);
            return new PropertyMapper(propChain);
        }

        public static IEnumerable<ColumnMeta> CreateColumnMetaChain<T, TProp>(Expression<Func<T, TProp>> propExp)
        {
            return CreatePropertyChain(propExp)
                .Select(p => DbMeta.Instance.GetColumn(p))
                .ToArray();
        }

        public static IEnumerable<PropertyInfo> CreatePropertyChain(Expression expression)
        {
            var list = new List<PropertyInfo>();

            var memberExpression = (expression as LambdaExpression).Body as MemberExpression;

            while (memberExpression != null)
            {
                list.Insert(0, memberExpression.Member as PropertyInfo);
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            return list;
        }
    }
}