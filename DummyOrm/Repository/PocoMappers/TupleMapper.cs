using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DummyOrm.Meta;

namespace DummyOrm.Repository.PocoMappers
{
    public class TupleMapper<T1, T2> : MultiPocoMapper
    {
        public TupleMapper(IDictionary<string, ColumnMeta> outputMappings)
            : base(outputMappings)
        {
        }

        public override object Map(IDataReader reader)
        {
            var objects = (object[])base.Map(reader);
            return new Tuple<T1, T2>(
                objects.OfType<T1>().FirstOrDefault(),
                objects.OfType<T2>().FirstOrDefault());
        }
    }
}