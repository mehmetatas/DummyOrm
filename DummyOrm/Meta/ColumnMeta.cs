using System;
using System.Reflection;
using DummyOrm.Sql;

namespace DummyOrm.Meta
{
    public class ColumnMeta
    {
        public TableMeta Table { get; set; }
        public PropertyInfo Property { get; set; }
        public string ColumnName { get; set; }
        public bool Identity { get; set; }
        public bool AutoIncrement { get; set; }
        public bool IsRefrence { get; set; }
        
        public Column Column { get; set; }

        public string QuotedName
        {
            get { return "[" + ColumnName + "]"; }
        }

        public object GetValue(object entity)
        {
            var value = Property.GetValue(entity);
            
            if (value == null || !IsRefrence)
            {
                return value;
            }

            var refTable = DbMeta.Instance.GetTable(Property.PropertyType);
            return refTable.IdColumn.GetValue(value);
        }

        public Type GetParamType()
        {
            if (!IsRefrence)
            {
                return Property.PropertyType;
            }

            var refTable = DbMeta.Instance.GetTable(Property.PropertyType);
            return refTable.IdColumn.GetParamType();
        }
    }
}