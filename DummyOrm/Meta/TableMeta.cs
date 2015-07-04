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

        public string QuotedName
        {
            get { return "[" + TableName + "]"; }
        }

        public ColumnMeta GetColumn(PropertyInfo propInf)
        {
            return Columns.First(c => c.Property == propInf);
        }
    }
}