using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DummyOrm.Meta;
using DummyOrm.Sql.QueryBuilders.Where;
using DummyOrm.Sql.QueryBuilders.Where.Expressions;
using DummyOrm.Sql.QueryBuilders.Where.ExpressionVisitors;
using DummyOrm.Utils;

namespace DummyOrm.Sql.QueryBuilders.Select
{
    /// <summary>
    /// Adapter: 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectQueryBuilder<T> : ISelectQueryBuilder<T>
    {
        private readonly TableMeta _tableMeta;
        private readonly HashSet<Column> _includeColumns;
        private readonly HashSet<Column> _excludeColumns;
        private readonly List<IWhereExpression> _whereExpressions;
        private readonly HashSet<OrderBy> _orderByColumns;
        private readonly HashSet<Join> _joins;
        private readonly Dictionary<string, ColumnMeta> _outputMappings; 
        private int _pageIndex;
        private int _pageSize;

        public SelectQueryBuilder()
        {
            _includeColumns = new HashSet<Column>();
            _excludeColumns = new HashSet<Column>();
            _whereExpressions = new List<IWhereExpression>();
            _orderByColumns = new HashSet<OrderBy>();
            _joins = new HashSet<Join>();
            _outputMappings = new Dictionary<string, ColumnMeta>();

            _tableMeta = DbMeta.Instance.GetTable<T>();
        }

        public ISelectQueryBuilder<T> Join<TOther>(Expression<Func<TOther, T, bool>> joinOn)
        {
            return Join(joinOn.Body, JoinType.Inner);
        }

        public ISelectQueryBuilder<T> LeftJoin<TOther>(Expression<Func<TOther, T, bool>> joinOn)
        {
            return Join(joinOn.Body, JoinType.Left);
        }

        public ISelectQueryBuilder<T> RightJoin<TOther>(Expression<Func<TOther, T, bool>> joinOn)
        {
            return Join(joinOn.Body, JoinType.Right);
        }

        public ISelectQueryBuilder<T> Join<T1, T2>(Expression<Func<T1, T2, bool>> joinOn)
        {
            return Join(joinOn.Body, JoinType.Inner);
        }

        public ISelectQueryBuilder<T> LeftJoin<T1, T2>(Expression<Func<T1, T2, bool>> joinOn)
        {
            return Join(joinOn.Body, JoinType.Left);
        }

        public ISelectQueryBuilder<T> RightJoin<T1, T2>(Expression<Func<T1, T2, bool>> joinOn)
        {
            return Join(joinOn.Body, JoinType.Right);
        }

        private ISelectQueryBuilder<T> Join(Expression joinOnBody, JoinType type)
        {
            var left = (((System.Linq.Expressions.BinaryExpression)joinOnBody).Left as MemberExpression).Member as PropertyInfo;
            var right = (((System.Linq.Expressions.BinaryExpression)joinOnBody).Right as MemberExpression).Member as PropertyInfo;

            var column1 = DbMeta.Instance.GetColumn(left);
            var column2 = DbMeta.Instance.GetColumn(right);

            _joins.Add(new Join
            {
                Type = type,
                Column1 = column1.Column,
                Column2 = column2.Column
            });

            return this;
        }

        public ISelectQueryBuilder<T> Include<TOther>(params Expression<Func<TOther, object>>[] props)
        {
            var columnMetas = props.Any()
                ? props.Select(prop => DbMeta.Instance.GetColumn(prop.GetPropertyInfo()))
                : DbMeta.Instance.GetTable<TOther>().Columns;
            
            foreach (var columnMeta in columnMetas)
            {
                IncludeColumn(columnMeta);
            }

            return this;
        }

        public ISelectQueryBuilder<T> Exclude<TOther>(params Expression<Func<TOther, object>>[] props)
        {
            var columnMetas = props.Any()
                ? props.Select(prop => DbMeta.Instance.GetColumn(prop.GetPropertyInfo()))
                : DbMeta.Instance.GetTable<TOther>().Columns;

            foreach (var columnMeta in columnMetas)
            {
                _excludeColumns.Add(columnMeta.Column);
            }

            return this;
        }

        public ISelectQueryBuilder<T> OrderBy<TOther>(bool desc, params Expression<Func<TOther, object>>[] props)
        {
            foreach (var prop in props)
            {
                var columnMeta = DbMeta.Instance.GetColumn(prop.GetPropertyInfo());
                _orderByColumns.Add(new OrderBy
                {
                    Column = columnMeta.Column,
                    Desc = desc
                });
            }
            return this;
        }

        public ISelectQueryBuilder<T> Where<TOther>(Expression<Func<TOther, bool>> filter)
        {
            return WhereInternal(filter);
        }

        public ISelectQueryBuilder<T> Where<T1, T2>(Expression<Func<T1, T2, bool>> filter)
        {
            return WhereInternal(filter);
        }

        public ISelectQueryBuilder<T> Where<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> filter)
        {
            return WhereInternal(filter);
        }

        public ISelectQueryBuilder<T> Where<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> filter)
        {
            return WhereInternal(filter);
        }

        public ISelectQueryBuilder<T> Where<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> filter)
        {
            return WhereInternal(filter);
        }

        private ISelectQueryBuilder<T> WhereInternal(Expression filter)
        {
            _whereExpressions.Add(WhereExpressionVisitor.Build(filter));
            return this;
        }

        public ISelectQueryBuilder<T> Top(int top)
        {
            return Page(0, top);
        }

        public ISelectQueryBuilder<T> Page(int page, int pageSize)
        {
            _pageIndex = page;
            _pageSize = pageSize;
            return this;
        }

        public SelectQuery Build()
        {
            var query = new SelectQuery
            {
                Table = _tableMeta.QuotedName,
                Joins = _joins,
                OrderByColumns = _orderByColumns,
                OutputMappings = _outputMappings,
                PageIndex = _pageIndex,
                PageSize = _pageSize
            };
            
            if (query.IsPagingQuery && !_orderByColumns.Any())
            {
                _orderByColumns.Add(new OrderBy
                {
                    Column = _tableMeta.IdColumn.Column
                });
            }

            if (_includeColumns.All(c => c.Table != _tableMeta.TableName))
            {
                foreach (var columnMeta in _tableMeta.Columns.Where(c => !_excludeColumns.Any(ec => ec.ColumnName == c.ColumnName && ec.Table == c.Table.TableName)))
                {
                    IncludeColumn(columnMeta);
                }
            }

            query.SelectColumns = _includeColumns;
            query.OutputMappings = _outputMappings;
            query.IsSimpleMapping = _includeColumns.Count == _tableMeta.Columns.Length &&
                                    _includeColumns.ToList().TrueForAll(c => c.Table == _tableMeta.TableName);

            if (_whereExpressions.Any())
            {
                var where = new LogicalExpression
                {
                    Operand1 = _whereExpressions[0],
                    Operator = SqlOperator.And
                };

                foreach (var whereExpression in _whereExpressions.Skip(1))
                {
                    where.Operand2 = whereExpression;
                    where = new LogicalExpression
                    {
                        Operand1 = where,
                        Operator = SqlOperator.And
                    };
                }

                query.Where = where.Operand1;
            }

            return query;
        }

        private void IncludeColumn(ColumnMeta columnMeta)
        {
            if (columnMeta.IsRefrence)
            {
                var refTable = DbMeta.Instance.GetTable(columnMeta.Property.PropertyType);
                _joins.Add(new Join
                {
                    Type = JoinType.Inner,
                    Column1 = refTable.IdColumn.Column,
                    Column2 = columnMeta.Column
                });
                foreach (var column in refTable.Columns)
                {
                    _includeColumns.Add(column.Column);
                    if (!_outputMappings.ContainsKey(column.Column.Alias))
                    {
                        _outputMappings.Add(column.Column.Alias, column);
                    }
                }
            }
            else
            {
                _includeColumns.Add(columnMeta.Column);
                if (!_outputMappings.ContainsKey(columnMeta.Column.Alias))
                {
                    _outputMappings.Add(columnMeta.Column.Alias, columnMeta);
                }
            }
        }
    }
}