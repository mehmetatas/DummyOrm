using System.Collections.Generic;
using System.Data;
using DummyOrm.Meta;

namespace DummyOrm.Repository.PocoMappers
{
    public class CustomPocoMapper : MultiPocoMapper
    {
        public CustomPocoMapper(IDictionary<string, ColumnMeta> outputMappings) : base(outputMappings)
        {
        }

        public override object Map(IDataReader reader)
        {
            var objects = (object[])base.Map(reader);
            return objects[0];
        }
    }
}