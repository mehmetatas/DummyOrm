using System;
using System.Collections.Generic;
using DummyOrm.Execution;
using DummyOrm.Meta;

namespace DummyOrm.Sql.QueryBuilders.Select.Graph
{
    public class SelectGraph
    {
        private readonly List<string> _joinKeys;
        private readonly List<string> _tableAliases;

        public SelectGraph()
        {
            _joinKeys = new List<string>();
            _tableAliases = new List<string>();
        }

        public bool JoinExists(string joinKey)
        {
            return _joinKeys.Contains(joinKey);
        }

        public void AddJoinKey(string joinKey)
        {
            _joinKeys.Add(joinKey);
        }

        public TableNode FromTable { get; private set; }

        public string GetTableAlias(string tableName)
        {
            var key = Char.ToString(Char.ToLowerInvariant(tableName[0]));

            var tmp = key;
            var i = 0;

            while (_tableAliases.Contains(tmp))
            {
                tmp = key + (++i);
            }

            _tableAliases.Add(tmp);

            return tmp;
        }

        public TableNode SetFromTable(TableMeta table)
        {
            FromTable = new TableNode(table, this);
            return FromTable;
        }

        public IPocoDeserializer CreateDeserializer()
        {
            var mappers = FromTable.BuildMappers();
            return new PocoDeserializer(FromTable.Table.Type, mappers);
        }
    }
}