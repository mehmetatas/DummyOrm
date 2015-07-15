using System;
using System.Collections.Generic;
using System.Reflection;
using DummyOrm.Meta;

namespace DummyOrm.Sql.Select
{
    public class Column
    {
        public ColumnMeta Meta { get; set; }
        public string Alias { get; set; }
        public Table Table { get; set; }
        public IList<PropertyInfo> PropertyChain { get; set; }

        public override string ToString()
        {
            return String.Format("[{0}] {1}.{2}", Meta.Table.TableName, Table.Alias, Alias);
        }
    }
}