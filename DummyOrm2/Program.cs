using DummyOrm2.Entities;
using DummyOrm2.Orm.Db;
using DummyOrm2.Orm.Meta;
using DummyOrm2.Orm.Sql.Select;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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
            var post = new Post { Id = 12 };

            using (var conn = new SqlConnection("Server=.;Database=TagKid2;uid=sa;pwd=123456"))
            {
                conn.Open();
                var db = new DbImpl(conn);

                Page<Like> page = null;

                var sw = new Stopwatch();
                sw.Start();
                for (var i = 0; i < 1; i++)
                {
                    page = db.Select<Like>()
                        .Join(l => l.User, u => new { u.Id, u.Username, u.Fullname })
                        .Join(l => l.Post, p => new { p.Id, p.Title })
                        .Join(l => l.Post.User, u => new { u.Id, u.Username, u.Fullname })
                        .Where(l =>
                            DateTime.Now.ToUniversalTime().ToLocalTime().ToUniversalTime() > l.User.JoinDate &&
                            userIds.Contains(l.User.Id) &&
                            l.Post.User.Username.StartsWith("taga".Substring(0, 2)) &&
                            l.Post.User != l.User)
                        .Where(l => l.Post.Id != post.Id)
                        .OrderBy(l => l.Post.User)
                        .OrderByDesc(l => l.LikedDate)
                        .Page(1, 1);
                }
                sw.Stop();

                Console.WriteLine(page.Items.Count());
                Console.WriteLine(page.TotalCount);
                Console.WriteLine(page.HasMore);

                Console.WriteLine(sw.ElapsedMilliseconds);
            }
            Console.ReadLine();
        }
    }
}
