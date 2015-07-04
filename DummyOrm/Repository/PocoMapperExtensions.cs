using System;
using System.Collections.Generic;
using System.Data;

namespace DummyOrm.Repository
{
    public static class PocoMapperExtensions
    {
        public static Page<T> Page<T>(this IPocoMapper pocoMapper, IDataReader reader, int page, int pageSize)
        {
            var items = new List<T>();
            var totalCount = 0;

            while (reader.Read())
            {
                totalCount = Convert.ToInt32(reader["__ROWCOUNT"]);
                var item = (T) pocoMapper.Map(reader);
                items.Add(item);
            }

            return new Page<T>(page, pageSize, totalCount, items);
        } 
    }
}