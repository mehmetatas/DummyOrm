using System;
using System.Data;
using System.Reflection;
using DummyOrm2.Orm.Dynamix;
using DummyOrm2.Orm.Sql.Select;

namespace DummyOrm2.Orm.Meta
{
    public class ColumnMeta
    {
        public TableMeta Table { get; set; }
        public PropertyInfo Property { get; set; }
        public DbType DbType { get; set; }
        public string ColumnName { get; set; }
        public bool Identity { get; set; }
        public bool AutoIncrement { get; set; }
        public bool IsRefrence { get; set; }
        public int DecimalPrecision { get; set; }
        public int StringLength { get; set; }
        public IGetterSetter GetterSetter { get; set; }
        public TableMeta ReferencedTable { get; set; }

        /// <summary>
        /// this = Like.Post
        /// FROM Like l
        /// JOIN Post p ON l.PostId = p.Id
        /// </summary>
        public Join CreateJoin(string leftTableAlias)
        {
            return new Join
            {
                LeftColumn = new Column
                {
                    Meta = this,
                    Table = new Table
                    {
                        Alias = leftTableAlias,
                        Meta = Table
                    }
                },
                RightColumn = new Column
                {
                    Meta = ReferencedTable.IdColumn,
                    Table = new Table
                    {
                        Alias = leftTableAlias + "_" + ReferencedTable.IdColumn.ColumnName,
                        Meta = ReferencedTable
                    }
                }
            };
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}", Table, ColumnName);
        }
    }
}