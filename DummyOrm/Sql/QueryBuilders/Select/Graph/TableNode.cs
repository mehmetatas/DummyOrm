using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DummyOrm.Execution;
using DummyOrm.Meta;

namespace DummyOrm.Sql.QueryBuilders.Select.Graph
{
    public class TableNode
    {
        private readonly SelectGraph _graph;

        public TableNode(TableMeta table, SelectGraph graph)
        {
            Table = table;
            _graph = graph;
            Alias = _graph.GetTableAlias(Name);
            Columns = table.Columns.Select(c => new ColumnNode(c, Alias)).ToList();
            Joins = new List<JoinEdge>();
        }

        public TableMeta Table { get; private set; }
        public string Alias { get; private set; }
        public List<ColumnNode> Columns { get; private set; }

        public List<JoinEdge> Joins { get; private set; }

        public string Name
        {
            get { return Table.TableName; }
        }

        public void Join<T, TProp>(Expression<Func<T, TProp>> propChain)
        {
            var chain = PropertyMapperFactory.CreateColumnMetaChain(propChain)
                .ToList();

            var currentTable = this;

            var key = "";
            foreach (var from in chain)
            {
                key += "_" + from.Table + "." + from.ColumnName;
                
                var to = from.GetReferencedTable().IdColumn;
                
                key += "_" + to.Table + "." + to.ColumnName;

                if (_graph.JoinExists(key))
                {
                    currentTable = currentTable.Joins
                        .First(j => j.Key == key)
                        .ToTable;
                }
                else
                {
                    var join = currentTable.Join(to, from);
                    join.Key = key;
                    _graph.AddJoinKey(key);
                    currentTable = join.ToTable;
                }
            }
        }

        public ColumnNode AddColumn(string propName)
        {
            return AddColumn(Table[propName]);
        }

        public ColumnNode AddColumn(ColumnMeta column)
        {
            var col = new ColumnNode(column, Alias);

            Columns.Add(col);

            return col;
        }

        public JoinEdge Join(ColumnMeta toColumn, ColumnMeta fromCol)
        {
            var edge = new JoinEdge
            {
                ToTable = new TableNode(toColumn.Table, _graph),
                ToColumn = toColumn,
                FromTable = this,
                FromColumn = fromCol,
                Type = JoinType.Inner
            };

            Joins.Add(edge);

            return edge;
        }

        public IDictionary<string, PropertyMapper> BuildMappers(Queue<ColumnMeta> chain = null)
        {
            if (chain == null)
            {
                chain = new Queue<ColumnMeta>();
            }

            var dic = Columns.ToDictionary(
                c => c.Alias,
                c => new PropertyMapper(new List<ColumnMeta>(chain)
                {
                    c.Column
                }));

            foreach (var join in Joins)
            {
                chain.Enqueue(join.FromColumn);

                var joinDic = join.ToTable.BuildMappers(chain);
                foreach (var kv in joinDic)
                {
                    dic.Add(kv.Key, kv.Value);
                }

                chain.Dequeue();
            }

            return dic;
        }
    }
}