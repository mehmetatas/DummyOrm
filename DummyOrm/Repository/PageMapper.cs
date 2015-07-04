using System.Collections.Generic;
using System.Data;

namespace DummyOrm.Repository
{
    public static class PageMapper
    {
        public static Page<T> Map<T>(IDataReader reader, IPocoMapper pocoMapper, int page, int pageSize)
        {
            var items = new List<T>();
            var totalCount = 0;

            while (reader.Read())
            {
                totalCount = reader.GetInt32(reader.FieldCount - 1);
                var item = (T) pocoMapper.Map(reader);
                items.Add(item);
            }

            return new Page<T>(page, pageSize, totalCount, items);
        } 
    }
}