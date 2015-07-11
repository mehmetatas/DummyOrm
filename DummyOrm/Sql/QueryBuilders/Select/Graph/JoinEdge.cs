using DummyOrm.Meta;

namespace DummyOrm.Sql.QueryBuilders.Select.Graph
{
    public class JoinEdge
    {
        public string Key { get; set; }
        public ColumnMeta FromColumn { get; set; }
        public ColumnMeta ToColumn { get; set; }
        public TableNode FromTable { get; set; }
        public TableNode ToTable { get; set; }
        public JoinType Type { get; set; }

        public string FromColumnName
        {
            get { return FromColumn.ColumnName; }
        }

        public string ToColumnName
        {
            get { return ToColumn.ColumnName; }
        }
    }
}