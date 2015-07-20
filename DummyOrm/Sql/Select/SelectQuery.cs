﻿using System.Linq.Expressions;
using DummyOrm.Dynamix.Impl;
using DummyOrm.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using DummyOrm.Sql.Where;
using DummyOrm.Sql.Where.ExpressionVisitors;

namespace DummyOrm.Sql.Select
{
    public class SelectQuery<T> : IWhereExpressionListener where T : class, new()
    {
        private readonly AliasContext _aliasCtx = new AliasContext();
        private readonly Dictionary<string, Table> _tables = new Dictionary<string, Table>();
        private readonly Dictionary<string, IEnumerable<ColumnMeta>> _outputMappings = new Dictionary<string, IEnumerable<ColumnMeta>>();

        public Table From { get; private set; }
        public IDictionary<string, Column> SelectColumns { get; private set; }
        public IDictionary<string, Join> Joins { get; private set; }
        public List<IWhereExpression> WhereExpressions { get; private set; }
        public List<OrderBy> OrderByColumns { get; private set; }

        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public bool IsPaging
        {
            get { return Page > 0 && PageSize > 0; }
        }

        public bool IsTop
        {
            get { return Page < 1 && PageSize > 0; }
        }

        public SelectQuery()
        {
            SelectColumns = new Dictionary<string, Column>();
            WhereExpressions = new List<IWhereExpression>();
            Joins = new Dictionary<string, Join>();
            OrderByColumns = new List<OrderBy>();

            var fromTableMeta = DbMeta.Instance.GetTable<T>();
            From = GetOrAddTable(fromTableMeta.TableName, fromTableMeta);

            foreach (var columnMeta in fromTableMeta.Columns)
            {
                var column = AddColumn(From, columnMeta);
                _outputMappings.Add(column.Alias, new[] { column.Meta });
            }
        }

        Column IWhereExpressionListener.RegisterColumn(IList<ColumnMeta> propChain)
        {
            var colMeta = propChain.Last();
            
            Table table;
            if (colMeta.Identity && propChain.Count > 1)
            {
                colMeta = propChain[propChain.Count - 2];
                table = EnsureJoined(propChain.Take(propChain.Count - 2));
            }
            else
            {
                table = EnsureJoined(propChain.Take(propChain.Count - 1));
            }

            return new Column
            {
                Meta = colMeta,
                Table = table
            };
        }

        public EntityDeserializer CreateDeserializer()
        {
            return new EntityDeserializer(From.Meta.Factory, _outputMappings);
        }

        public void AddColumn(IList<ColumnMeta> propChain)
        {
            var colMeta = propChain.Last();
            var table = EnsureJoined(propChain.Take(propChain.Count - 1));
            var column = AddColumn(table, colMeta);

            if (!_outputMappings.ContainsKey(column.Alias))
            {
                _outputMappings.Add(column.Alias, propChain);
            }
        }

        public void Join(IList<ColumnMeta> props)
        {
            EnsureJoined(props);
        }

        public void Where(Expression<Func<T, bool>> filter)
        {
            var whereExp = WhereExpressionVisitor.Build(filter, this);
            WhereExpressions.Add(whereExp);
        }

        public void OrderBy(IList<ColumnMeta> propChain, bool desc)
        {
            var colMeta = propChain.Last();
            var table = EnsureJoined(propChain.Take(propChain.Count - 1));
            var column = AddColumn(table, colMeta);

            if (OrderByColumns.All(c => c.Column.Alias != column.Alias))
            {
                OrderByColumns.Add(new OrderBy
                {
                    Desc = desc,
                    Column = column
                });
            }
        }

        public void SetPage(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public void Top(int top)
        {
            Page = -1;
            PageSize = top;
        }

        private Column AddColumn(Table table, ColumnMeta colMeta)
        {
            var colAlias = String.Format("{0}_{1}", table.Alias, colMeta.ColumnName);

            if (SelectColumns.ContainsKey(colAlias))
            {
                return SelectColumns[colAlias];
            }

            var column = new Column
            {
                Alias = colAlias,
                Meta = colMeta,
                Table = table
            };

            SelectColumns.Add(column.Alias, column);

            return column;
        }

        private static string GetTableKey(IEnumerable<ColumnMeta> propChain)
        {
            var u = "";
            var key = "";

            foreach (var leftCol in propChain)
            {
                if (!leftCol.IsRefrence)
                {
                    break;
                }

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

        /// <summary>
        /// Ensures a valid join for the property chain exists in the query and 
        /// returns the Table object created for the property.
        /// </summary>
        /// <param name="props">The props.</param>
        /// <returns></returns>
        private Table EnsureJoined(IEnumerable<ColumnMeta> props)
        {
            var leftTable = From;
            var i = 0;
            foreach (var leftColMeta in props)
            {
                if (!leftColMeta.IsRefrence)
                {
                    break;
                }

                var rightTableMeta = leftColMeta.ReferencedTable;
                var rightTableKey = GetTableKey(props.Take(++i));
                var rightTable = GetOrAddTable(rightTableKey, rightTableMeta);

                var joinKey = String.Format("{0}_{1}", leftTable.Alias, rightTable.Alias);

                if (!Joins.ContainsKey(joinKey))
                {
                    var leftColumn = new Column
                    {
                        Table = leftTable,
                        Meta = leftColMeta
                    }; // AddColumn(leftTable, leftColMeta);

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
