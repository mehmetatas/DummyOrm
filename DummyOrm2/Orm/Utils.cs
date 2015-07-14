using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using DummyOrm2.Orm.Meta;

namespace DummyOrm2.Orm
{
    public static class Utils
    {
        private static readonly IDictionary<Type, DbType> TypeMap = new ReadOnlyDictionary<Type, DbType>(new Dictionary<Type, DbType>
        {
            { typeof(byte), DbType.Byte },
            { typeof(sbyte), DbType.SByte },
            { typeof(short), DbType.Int16 },
            { typeof(ushort), DbType.UInt16 },
            { typeof(int), DbType.Int32 },
            { typeof(uint), DbType.UInt32 },
            { typeof(long), DbType.Int64 },
            { typeof(ulong), DbType.UInt64 },
            { typeof(float), DbType.Single },
            { typeof(double), DbType.Double },
            { typeof(decimal), DbType.Decimal },
            { typeof(bool), DbType.Boolean },
            { typeof(char), DbType.StringFixedLength },
            { typeof(string), DbType.String },
            { typeof(byte[]), DbType.Binary },
            { typeof(Guid), DbType.Guid },
            { typeof(DateTime), DbType.DateTime2 }
        });

        public static bool IsReferenceProperty(this PropertyInfo propInf)
        {
            var type = propInf.PropertyType.AsNonNullable();
            return !TypeMap.ContainsKey(type) && !type.IsEnum;
        }

        public static bool IsColumnProperty(this PropertyInfo propInf)
        {
            var propType = propInf.PropertyType;
            return !typeof(IEnumerable).IsAssignableFrom(propType) ||
                propType == typeof(string) ||
                propType == typeof(byte[]);
        }

        public static Type AsNonNullable(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0];
            }

            return type;
        }

        public static DbType GetDbType(this Type type)
        {
            type = type.AsNonNullable();

            if (type.IsEnum)
            {
                return DbType.Int32;
            }

            if (TypeMap.ContainsKey(type))
            {
                return TypeMap[type];
            }

            throw new NotSupportedException("Unsupported property type: " + type);
        }

        public static PropertyInfo GetPropertyInfo<T, TProp>(this Expression<Func<T, TProp>> propExpression)
        {
            return propExpression.GetMemberExpression().Member as PropertyInfo;
        }

        public static PropertyInfo GetPropertyInfo(this LambdaExpression propExpression)
        {
            return propExpression.GetMemberExpression().Member as PropertyInfo;
        }

        public static MemberExpression GetMemberExpression<T, TProp>(this Expression<Func<T, TProp>> propExpression)
        {
            return GetMemberExpression((LambdaExpression) propExpression);
        }

        public static MemberExpression GetMemberExpression(this LambdaExpression propExpression)
        {
            return propExpression.Body is UnaryExpression
                ? (MemberExpression)((UnaryExpression)propExpression.Body).Operand
                : (MemberExpression)propExpression.Body;
        }

        public static List<ColumnMeta> GetPropertyChain(this LambdaExpression propExpression, bool onlyRef = true)
        {
            return propExpression.GetMemberExpression().GetPropertyChain(onlyRef);
        }

        public static List<ColumnMeta> GetPropertyChain<T, TProp>(this Expression<Func<T, TProp>> propExpression, bool onlyRef = true)
        {
            return propExpression.GetMemberExpression().GetPropertyChain(onlyRef);
        }

        public static List<ColumnMeta> GetPropertyChain(this MemberExpression memberExpression, bool onlyRef = true)
        {
            var chain = new List<ColumnMeta>();

            while (memberExpression != null)
            {
                var propInf = (PropertyInfo)memberExpression.Member;
                if (propInf.IsReferenceProperty() || !onlyRef)
                {
                    chain.Insert(0, DbMeta.Instance.GetColumn(propInf));
                }
                memberExpression = memberExpression.Expression as MemberExpression;
            }
            return chain;
        }
    }
}