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
            DbMeta.Instance
                .Register<User>()
                .Register<Post>()
                .Register<Like>();

            var query = new QueryImpl<Like>();

            //query.Join(l => l.Post);
            //query.Join(l => l.Post.User);
            //query.Join(l => l.Post, p => p.User);
            //query.Join(l => l.Post, p => new
            //{
            //    p.Title,
            //    p.User
            //});

            var selectQuery = query.Build();
            var cmd = new SelectSqlCommandBuilderImpl().Build(selectQuery);

            Console.WriteLine(cmd.CommandText);
            
            Console.ReadLine();
        }
    }
}
