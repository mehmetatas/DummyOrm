using System;
using System.Linq;
using System.Reflection;

namespace DummyOrm.Meta
{
    public class TableMeta
    {
        public Type Type { get; set; }
        public ColumnMeta[] Columns { get; set; }
        public string TableName { get; set; }
        public bool AssociationTable { get; set; }
        public ColumnMeta IdColumn { get; set; }

        public Func<object> Factory { get; set; }

        public ColumnMeta this[string colName]
        {
            get { return Columns.First(c => c.ColumnName == colName); }
        }

        public ColumnMeta GetColumn(PropertyInfo propInf)
        {
            return Columns.First(c => c.Property == propInf);
        }

        public override string ToString()
        {
            return TableName;
        }
    }
}