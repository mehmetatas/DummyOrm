using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using DummyOrm2.Entities;
using DummyOrm2.Orm.Db;
using DummyOrm2.Orm.Meta;
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

            var userIds = new long[] { 1, 2, 3 };

            using (var conn = new SqlConnection("Server=.;Database=TagKid2;uid=sa;pwd=123456"))
            {
                conn.Open();
                var db = new DbImpl(conn);

                var page = db.Select<Like>()
                    .Join(l => l.User)
                    .Join(l => l.Post)
                    .Join(l => l.Post.User)
                    //.Where(l =>
                    //    DateTime.Now.ToUniversalTime().ToLocalTime().ToUniversalTime() > l.User.JoinDate &&
                    //    userIds.Contains(l.User.Id) &&
                    //    l.Post.User.Username.StartsWith("taga".Substring(0, 2)) &&
                    //    l.Post.User != l.User)
                    .Where(l => l.Post.Id == 3)
                    .OrderBy(l => l.Post.User)
                    .OrderByDesc(l => l.LikedDate)
                    .Page(1, 1);

                Console.WriteLine(page.Items.Count());
                Console.WriteLine(page.TotalCount);
                Console.WriteLine(page.HasMore);
            }
            Console.ReadLine();
        }
    }
}
