using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DummyOrm2.Orm.Meta;
using DummyOrm2.Orm.Sql;
using DummyOrm2.Orm.Sql.Select;

namespace DummyOrm2.Orm.Db
{
    public interface IQuery<T> : IWhereQuery<T> where T : class, new()
    {
        IQuery<T> Join<TProp>(Expression<Func<T, TProp>> refProp,
            Expression<Func<TProp, object>> include = null) where TProp : class, new();

        IQuery<T> Include(Expression<Func<T, object>> propExpression);
    }

    public class QueryImpl<T> : IQuery<T>, ISelectQueryBuilder<T> where T : class, new()
    {
        private readonly SelectQuery<T> _query;

        public QueryImpl()
        {
            _query = new SelectQuery<T>();
        }

        public IQuery<T> Join<TProp>(Expression<Func<T, TProp>> refProp, Expression<Func<TProp, object>> include = null) where TProp : class ,new()
        {
            var propChain = refProp.GetPropertyChain();

            // Build join
            _query.Join(propChain);

            if (include == null)
            {
                // No columns specified Include all properties of joined type
                Include(refProp.GetMemberExpression());
            }
            else
            {
                if (include.Body is NewExpression)
                {
                    // Include multiple properties
                    Include(include.Body as NewExpression, propChain);
                }
                else
                {
                    // Include specified property only
                    Include(include.GetMemberExpression(), propChain);
                }
            }

            return this;
        }

        public IQuery<T> Include(Expression<Func<T, object>> propExpression)
        {
            if (propExpression.Body is NewExpression)
            {
                Include(propExpression.Body as NewExpression);
            }
            else
            {
                Include(propExpression.GetMemberExpression());
            }
            return this;
        }

        private void Include(NewExpression newExp, List<PropertyInfo> rootChain = null)
        {
            var includedProps = newExp.Arguments.Cast<MemberExpression>();
            foreach (var includeProp in includedProps)
            {
                Include(includeProp, rootChain);
            }
        }

        private void Include(MemberExpression memberExp, List<PropertyInfo> rootChain = null)
        {
            var list = rootChain == null
                        ? new List<PropertyInfo>()
                        : new List<PropertyInfo>(rootChain);

            list.AddRange(memberExp.GetPropertyChain(false));
            _query.AddColumn(list);
            
            var prop = list.Last();
            var colMeta = DbMeta.Instance.GetColumn(prop);
            if (colMeta.IsRefrence)
            {
                foreach (var columnMeta in colMeta.ReferencedTable.Columns)
                {
                    _query.AddColumn(new List<PropertyInfo>(list) { columnMeta.Property });
                }
            }
        }

        public IWhereQuery<T> Where(Expression<Func<T, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public IOrderByQuery<T> OrderBy(Expression<Func<T, object>> props)
        {
            throw new NotImplementedException();
        }

        public IOrderByQuery<T> OrderByDesc(Expression<Func<T, object>> props)
        {
            throw new NotImplementedException();
        }

        public T FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public IList<T> ToList()
        {
            throw new NotImplementedException();
        }

        public Page<T> Page(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Page<T> Top(int top)
        {
            throw new NotImplementedException();
        }

        public SelectQuery<T> Build()
        {
            if (!_query.SelectColumns.Any())
            {
                var fromTable = _query.From.Meta;
                foreach (var columnMeta in fromTable.Columns)
                {
                    _query.AddColumn(new[] { columnMeta.Property });
                }
            }
            return _query;
        }
    }
}