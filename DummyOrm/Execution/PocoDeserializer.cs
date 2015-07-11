using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DummyOrm.Meta;

namespace DummyOrm.Execution
{
    public interface IPocoDeserializer
    {
        object Deserialize(IDataReader reader);
    } 

    public class PocoDeserializer : IPocoDeserializer
    {
        private readonly TableMeta _tableMeta;
        private readonly IDictionary<string, PropertyMapper> _propMappers;

        public PocoDeserializer(Type type, IDictionary<string, PropertyMapper> propertyMappers)
        {
            _tableMeta = DbMeta.Instance.GetTable(type);
            _propMappers = propertyMappers;
        }

        public object Deserialize(IDataReader reader)
        {
            var obj = _tableMeta.Factory();

            foreach (var propMapper in _propMappers)
            {
                var alias = propMapper.Key;
                var mapper = propMapper.Value;

                var value = reader[alias];

                if (value == null || value == DBNull.Value)
                {
                    continue;
                }

                mapper.Map(obj, value);
            }

            return obj;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var mapper in _propMappers)
            {
                sb.Append(mapper.Key);
                sb.Append("\t");
                sb.AppendLine(mapper.Value.ToString());
            }
            return sb.ToString();
        }
    }
}