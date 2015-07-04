using System;
namespace DummyOrm.Sql
{
    public class Column
    {
        public string Table { get; set; }
        public string ColumnName { get; set; }

        private string _fullname;
        public string Fullname
        {
            get { return _fullname ?? (_fullname = String.Format("[{0}].[{1}]", Table, ColumnName)); }
        }

        private string _alias;
        public string Alias
        {
            get { return _alias ?? (_alias = String.Format("{0}_{1}", Table, ColumnName)); }
        }
    }
}