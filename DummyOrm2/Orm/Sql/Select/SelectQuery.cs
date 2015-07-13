using DummyOrm2.Orm.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DummyOrm2.Orm.Sql.Select
{
    public class SelectQuery<T>
    {
        private readonly AliasContext _aliasCtx = new AliasContext();
        private readonly Dictionary<string, Table> _tables = new Dictionary<string, Table>();

        public Table From { get; private set; }
        public IDictionary<string, Column> Columns { get; private set; }
        public IDictionary<string, Join> Joins { get; private set; }
        public List<OrderBy> OrderByColumns { get; private set; }

        public SelectQuery()
        {
            Columns = new Dictionary<string, Column>();
            Joins = new Dictionary<string, Join>();
            OrderByColumns = new List<OrderBy>();

            var fromTableMeta = DbMeta.Instance.GetTable<T>();
            From = GetOrAddTable(fromTableMeta.TableName, fromTableMeta);
        }

        private static string GetTableKey(IEnumerable<PropertyInfo> propChain)
        {
            var u = "";
            var key = "";

            foreach (var prop in propChain)
            {
                var leftCol = DbMeta.Instance.GetColumn(prop);
                var rightCol = leftCol.ReferencedTable.IdColumn;

                key += u + String.Format("{0}{1}_{2}{3}", leftCol.Table.TableName, leftCol.ColumnName, rightCol.Table.TableName, rightCol.ColumnName);

                u = "_";
            }

            return key;
        }

        private Table GetOrAddTable(string tableKey, TableMeta tableMeta)
        {
            if (_tables.ContainsKey(tableKey))
            {
                return _tables[tableKey];
            }

            var alias = _aliasCtx.GetAlias(tableMeta.TableName);

            var table = new Table
            {
                Alias = alias,
                Meta = tableMeta
            };

            _tables.Add(tableKey, table);

            return table;
        }

        public Column AddColumn(IList<PropertyInfo> propChain)
        {
            var table = EnsureTable(propChain);
            var prop = propChain.Last();
            return AddColumn(table, prop);
        }

        private Column AddColumn(Table table, PropertyInfo prop)
        {
            var colMeta = DbMeta.Instance.GetColumn(prop);

            if (colMeta.IsRefrence)
            {
                colMeta = colMeta.ReferencedTable.IdColumn;
            }

            return AddColumn(table, colMeta);
        }

        private Column AddColumn(Table table, ColumnMeta colMeta)
        {
            var colAlias = String.Format("{0}_{1}", table.Alias, colMeta.ColumnName);

            if (Columns.ContainsKey(colAlias))
            {
                return Columns[colAlias];
            }

            var column = new Column
            {
                Alias = colAlias,
                Meta = colMeta,
                Table = table
            };

            Columns.Add(colAlias, column);

            return column;
        }

        public Table Join(IList<PropertyInfo> props)
        {
            return EnsureTable(props);
        }

        private Table EnsureTable(IList<PropertyInfo> props)
        {
            var leftTable = From;

            for (var i = 0; i < props.Count; i++)
            {
                var leftColMeta = DbMeta.Instance.GetColumn(props[i]);

                if (!leftColMeta.IsRefrence)
                {
                    continue;
                }

                var rightTableMeta = leftColMeta.ReferencedTable;
                var rightTableKey = GetTableKey(props.Take(i + 1));
                var rightTable = GetOrAddTable(rightTableKey, rightTableMeta);

                    var joinKey = String.Format("{0}_{1}", leftTable.Alias, rightTable.Alias);

                    if (!Joins.ContainsKey(joinKey))
                    {
                        var leftColumn = AddColumn(leftTable, leftColMeta);
                        var rightColumn = AddColumn(rightTable, rightTableMeta.IdColumn);

                        Joins.Add(joinKey, new Join
                        {
                            LeftColumn = leftColumn,
                            RightColumn = rightColumn,
                            Type = JoinType.Inner
                        });
                    }

                leftTable = rightTable;
            }

            return leftTable; 
        }

        class AliasContext
        {
            private readonly List<string> _aliases = new List<string>();

            public string GetAlias(string key)
            {
                var root = Char.ToString(Char.ToLowerInvariant(key[0]));

                var i = 0;
                var alias = root;
                while (_aliases.Contains(alias))
                {
                    alias = root + (++i);
                }

                _aliases.Add(alias);

                return alias;
            }
        }
    }
}
