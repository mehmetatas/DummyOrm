using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

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
            return !TypeMap.ContainsKey(propInf.PropertyType);
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

        public static string GetJoinKey<T, TProp>(this Expression<Func<T, TProp>> propChain)
        {
            var key = new StringBuilder();

            var memberExpression = propChain.Body as MemberExpression;

            while (memberExpression != null)
            {
                var propInf = memberExpression.Member as PropertyInfo;
                if (propInf.IsReferenceProperty())
                {
                    key.Insert(0, propInf.Name);
                    key.Insert(0, ".");
                }
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            key.Insert(0, typeof(T).Name);

            return key.ToString();
        }

        public static List<Type> GetTypeChain<T, TProp>(this Expression<Func<T, TProp>> propChain)
        {
            var types = new List<Type>();

            var memberExpression = propChain.Body as MemberExpression;

            while (memberExpression != null)
            {
                var propInf = memberExpression.Member as PropertyInfo;
                if (propInf.IsReferenceProperty())
                {
                    types.Insert(0, propInf.PropertyType);
                }
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            types.Insert(0, typeof(T));

            return types;
        }
    }
}