using System;
using DummyOrm2.Orm.Meta;

namespace DummyOrm2.Orm.Sql.Select
{
    public class Column
    {
        public ColumnMeta Meta { get; set; }
        public string Alias { get; set; }
        public Table Table { get; set; }

        public override string ToString()
        {
            return String.Format("[{0}] {1}.{2}", Meta.Table.TableName, Table.Alias, Alias);
        }
    }
}