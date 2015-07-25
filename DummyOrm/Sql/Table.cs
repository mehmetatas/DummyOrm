using System;
using DummyOrm.Meta;

namespace DummyOrm.Sql
{
    public class Table
    {
        public TableMeta Meta { get; set; }
        public string Alias { get; set; }

        public override string ToString()
        {
            return String.Format("[{0}] {1}", Meta.TableName, Alias);
        }
    }
}