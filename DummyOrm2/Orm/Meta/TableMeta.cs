using System;

namespace DummyOrm2.Orm.Meta
{
    public class TableMeta
    {
        public Type Type { get; set; }
        public ColumnMeta[] Columns { get; set; }
        public string TableName { get; set; }
        public bool AssociationTable { get; set; }
        public ColumnMeta IdColumn { get; set; }

        public AssociationMeta[] Associations { get; set; }

        public Func<object> Factory { get; set; }
        
        public override string ToString()
        {
            return TableName;
        }
    }
}