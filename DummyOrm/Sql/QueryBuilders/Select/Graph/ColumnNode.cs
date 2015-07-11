using DummyOrm.Meta;

namespace DummyOrm.Sql.QueryBuilders.Select.Graph
{
    public class ColumnNode
    {
        public ColumnNode(ColumnMeta columnMeta, string tableAlias)
        {
            Column = columnMeta;
            TableAlias = tableAlias;
            Alias = TableAlias + "_" + Column.ColumnName;
        }

        public ColumnMeta Column { get; private set; }
        public string TableAlias { get; private set; }
        public string Alias { get; private set; }

    }
}