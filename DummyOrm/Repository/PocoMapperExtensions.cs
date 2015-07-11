using System;
using System.Collections.Generic;
using System.Data;
using DummyOrm.Execution;

namespace DummyOrm.Repository
{
    public static class PocoMapperExtensions
    {
        public static Page<T> Page<T>(this IPocoDeserializer pocoMapper, IDataReader reader, int page, int pageSize)
        {
            var items = new List<T>();
            var totalCount = 0;

            while (reader.Read())
            {
                totalCount = Convert.ToInt32(reader["__ROWCOUNT"]);
                var item = (T) pocoMapper.Deserialize(reader);
                items.Add(item);
            }

            return new Page<T>(page, pageSize, totalCount, items);
        } 
    }
}