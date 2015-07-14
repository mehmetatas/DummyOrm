using System.Linq;
using System.Reflection;
using DummyOrm2.Entities;
using DummyOrm2.Orm.Db;
using DummyOrm2.Orm.Meta;
using DummyOrm2.Orm.Sql;
using System;

namespace DummyOrm2
{
    class Program
    {
        static void Main(string[] args)
        {
            var entityClasses = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace == "DummyOrm2.Entities" && t.IsClass);

            foreach (var entityClass in entityClasses)
            {
                DbMeta.Instance.Register(entityClass);
            }

            var query = new QueryImpl<Notification>();

            query.Join(n => n.FromUser);
            query.Join(n => n.ToUser);
            query.Join(n => n.Post);
            query.Join(n => n.Post.User);

            //var query = new QueryImpl<Like>();

            //query.Join(l => l.Post);
            //query.Include(l => l.Post);
            //query.Include(l => l.Post.Title);
            //query.Join(l => l.User);
            //query.Join(l => l.Post.User);
            //query.Join(l => l.Post, p => p.Title);
            //query.Join(l => l.Post, p => new
            //{
            //    p.Title,
            //    p.User.Username
            //});

            var selectQuery = query.Build();
            var cmd = new SqlServerSelectSqlCommandBuilderImpl().Build(selectQuery);

            Console.WriteLine(cmd.CommandText);
            
            Console.ReadLine();
        }
    }
}
