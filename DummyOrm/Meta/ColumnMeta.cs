using DummyOrm.Execution;
using System;
using System.Reflection;

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
        public IGetterSetter GetterSetter { private get; set; }
        
        public object GetValue(object entity)
        {
            return GetterSetter.Get(entity);
        }

        public void SetValue(object entity, object value)
        {
            GetterSetter.Set(entity, value);
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

        public TableMeta GetReferencedTable()
        {
            return DbMeta.Instance.GetTable(Property.PropertyType);
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}", Table, ColumnName);
        }
    }
}