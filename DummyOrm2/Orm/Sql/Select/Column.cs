using DummyOrm2.Orm.Meta;

namespace DummyOrm2.Orm.Sql.Select
{
    public class Column
    {
        public ColumnMeta Meta { get; set; }
        public string Alias { get; set; }
        public Table Table { get; set; }
    }
}