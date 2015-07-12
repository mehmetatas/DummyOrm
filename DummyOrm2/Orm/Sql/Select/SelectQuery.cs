using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DummyOrm2.Orm.Meta;

namespace DummyOrm2.Orm.Sql.Select
{
    public class SelectQuery
    {
        private readonly Dictionary<string, string> _tableAliases = new Dictionary<string, string>();

        public Table From { get; set; }
        public List<Column> Columns { get; set; }
        public Dictionary<string, Join> Joins { get; set; }
        public List<OrderBy> OrderBy { get; set; }
        
        public Column AddColumn(MemberExpression memberExpression)
        {
            EnsureJoins(memberExpression);

            return null;
        }

        public Column GetColumn(MemberExpression memberExpression)
        {
            EnsureJoins(memberExpression);

            return null;
        }

        public void EnsureJoins(MemberExpression memberExpression)
        {
            var props = new List<PropertyInfo>();
            var joinNodes = new List<string>();

            while (memberExpression != null)
            {
                var propInf = (PropertyInfo)memberExpression.Member;
                if (propInf.IsReferenceProperty())
                {
                    props.Insert(0, propInf);
                    joinNodes.Insert(0, propInf.Name);
                }
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            joinNodes.Add(props[0].ReflectedType.Name);

            // like.Post.User.Detail
            for (var i = 0; i < props.Count; i++)
            {
                var leftAlias = String.Join("_", joinNodes.Take(i + 1));
                var joinKey = String.Join("_", joinNodes.Take(i + 2));

                if (Joins.ContainsKey(joinKey))
                {
                    continue;
                }

                var leftCol = DbMeta.Instance.GetColumn(props[i]);

                var join = leftCol.CreateJoin(leftAlias);

                Joins.Add(joinKey, join);
            }
        }
    }
}
