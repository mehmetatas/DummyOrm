using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DummyOrm.Meta;

namespace DummyOrm.Dynamix.Impl
{
    public class EntityDeserializer : IPocoDeserializer
    {
        private readonly Func<object> _factory;
        private readonly IDictionary<string, IEnumerable<ColumnMeta>> _propertyChain;

        public EntityDeserializer(Func<object> factory, IDictionary<string, IEnumerable<ColumnMeta>> propertyChain)
        {
            _factory = factory;
            _propertyChain = propertyChain;
        }

        public object Deserialize(IDataReader reader)
        {
            var entity = _factory();

            foreach (var kv in _propertyChain)
            {
                var value = reader[kv.Key];

                if (value == null || value == DBNull.Value)
                {
                    continue;
                }

                var chain = kv.Value;

                ColumnMeta lastProp = null;
                var tmp = entity;
                foreach (var columnMeta in chain)
                {
                    lastProp = columnMeta;

                    if (!columnMeta.IsRefrence)
                    {
                        continue;
                    }

                    var refObj = columnMeta.GetterSetter.Get(tmp);

                    if (refObj == null)
                    {
                        refObj = columnMeta.ReferencedTable.Factory();
                        columnMeta.GetterSetter.Set(tmp, refObj);
                    }

                    tmp = refObj;
                }

                if (lastProp.IsRefrence)
                {
                    lastProp.ReferencedTable.IdColumn.GetterSetter.Set(tmp, value);
                }
                else
                {
                    lastProp.GetterSetter.Set(tmp, value);
                }
            }

            return entity;
        }
    }

    public class ModelDerializer : IPocoDeserializer
    {
        private readonly Func<object> _factory;
        private readonly IDictionary<string, ISetter> _setters;

        public ModelDerializer(Func<object> factory, IDictionary<string, ISetter> setters)
        {
            _factory = factory;
            _setters = setters;
        }

        public object Deserialize(IDataReader reader)
        {
            var entity = _factory();

            foreach (var setter in _setters)
            {
                var value = reader[setter.Key];
                
                if (value == null || value == DBNull.Value)
                {
                    continue;
                }

                setter.Value.Set(entity, value);
            }

            return entity;
        }
    }

    public static class PocoDeserializer
    {
        private static readonly Hashtable DefaultDeserializers = new Hashtable();

        public static IPocoDeserializer GetDefault<T>() where T : class,new()
        {
            return GetDefault(typeof(T));
        }

        public static void RegisterEntity(TableMeta tableMeta)
        {
            var propChain = tableMeta.Columns.ToDictionary(c => c.ColumnName, c => (IEnumerable<ColumnMeta>)new[] { c });

            var deserializer = new EntityDeserializer(tableMeta.Factory, propChain);

            DefaultDeserializers.Add(tableMeta.Type, deserializer);
        }

        public static void RegisterModel<T>() where T : class,new()
        {
            RegisterModel(typeof(T));
        }

        public static void RegisterModel(Type type)
        {
            var setters = type.GetProperties()
                .ToDictionary(prop => prop.Name, prop => (ISetter) GetterSetter.Create(prop));
            
            var factory = PocoFactory.CreateFactory(type);
            
            var deserializer = new ModelDerializer(factory, setters);
            
            DefaultDeserializers.Add(type, deserializer);
        }

        public static IPocoDeserializer GetDefault(Type type)
        {
            return (IPocoDeserializer)DefaultDeserializers[type];
        }
    }
}