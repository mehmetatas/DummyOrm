using System;
using System.Data;
using System.Reflection;
using DummyOrm.Dynamix;

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
        public ParameterMeta ParameterMeta { get; set; }
        public TableMeta ReferencedTable { get; set; }
        public IGetterSetter GetterSetter { get; set; }
        public IAssociationLoader Loader { get; set; }

        public override string ToString()
        {
            return String.Format("{0}.{1}", Table, ColumnName);
        }
    }

    public class ParameterMeta
    {
        public DbType DbType { get; set; }
        public byte DecimalPrecision { get; set; }
        public int StringLength { get; set; }
    }
}