using DummyOrm2.Orm.Meta;

namespace DummyOrm2.Orm.Sql.Select
{
    public class Table
    {
        public TableMeta Meta { get; set; }
        public string Alias { get; set; }
    }
}