using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DummyOrm.Meta;

namespace DummyOrm.Repository.PocoMappers
{
    public abstract class MultiPocoMapper : IPocoMapper
    {
        private readonly IDictionary<string, ColumnMeta> _outputMappings;

        protected MultiPocoMapper(IDictionary<string, ColumnMeta> outputMappings)
        {
            _outputMappings = outputMappings;
        }

        public virtual object Map(IDataReader reader)
        {
            var objects = new Dictionary<Type, object>();

            foreach (var outputMapping in _outputMappings)
            {
                Map(reader, outputMapping.Key, outputMapping.Value, objects);
            }

            return objects.Values.ToArray();
        }

        private static void Map(IDataReader reader, string alias, ColumnMeta columnMeta, IDictionary<Type, object> objects)
        {
            var value = reader[alias];

            if (value == DBNull.Value)
            {
                return;
            }

            var obj = GetObject(columnMeta.Table.Type, objects);

            var prop = columnMeta.Property;

            value = EnsureValueType(prop, value);

            prop.SetValue(obj, value);
        }

        private static object GetObject(Type type, IDictionary<Type, object> objects)
        {
            object obj;

            if (objects.ContainsKey(type))
            {
                obj = objects[type];
            }
            else
            {
                obj = Activator.CreateInstance(type);
                objects.Add(type, obj);
            }

            return obj;
        }

        private static object EnsureValueType(PropertyInfo prop, object value)
        {
            var propType = prop.PropertyType;

            if (propType.IsGenericType &&
                propType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propType = propType.GetGenericArguments()[0];
            }

            value = propType.IsEnum
                ? Enum.ToObject(propType, value)
                : Convert.ChangeType(value, propType);

            return value;
        }
    }
}