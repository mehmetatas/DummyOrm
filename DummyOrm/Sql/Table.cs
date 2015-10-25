using DummyOrm.Meta;

namespace DummyOrm.Sql
{
    public class Table
    {
        public TableMeta Meta { get; set; }
        public string Alias { get; set; }

        public override string ToString()
        {
            return $"[{Meta.TableName}] {Alias}";
        }
    }
}