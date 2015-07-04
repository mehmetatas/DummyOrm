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

        public string QuotedName
        {
            get { return "[" + ColumnName + "]"; }
        }

        public Column Column { get; set; }
    }
}