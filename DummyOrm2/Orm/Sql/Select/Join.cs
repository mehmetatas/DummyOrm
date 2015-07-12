namespace DummyOrm2.Orm.Sql.Select
{
    public class Join
    {
        public string Key
        {
            get { return RightColumn.Table.Alias; }
        }

        public Column RightColumn { get; set; }
        public Column LeftColumn { get; set; }
        public JoinType Type { get; set; }
    }
}