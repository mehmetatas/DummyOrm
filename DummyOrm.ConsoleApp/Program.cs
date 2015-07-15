using DummyOrm.ConsoleApp.Entities;
using DummyOrm.Db;
using DummyOrm.Db.Impl;
using DummyOrm.Meta;
using DummyOrm.Sql.Select;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace DummyOrm.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var entityClasses = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace == "DummyOrm.ConsoleApp.Entities" && t.IsClass);

            foreach (var entityClass in entityClasses)
            {
                DbMeta.Instance.Register(entityClass);
            }

            DbMeta.Instance.BuildRelations();

            SelectList();
            SimpleCrudTestsAssociationEntity();
            SimpleCrudTestsEntity();
            SelectTests();

            Console.ReadLine();
        }

        private static void SelectList()
        {
            using (var conn = OpenConnection())
            {
                var db = new DbImpl(conn);

                var posts = db.Select<Post>().ToList();

                db.Load(posts, p => p.Tags);

                foreach (var post in posts.Where(p => p.Tags != null))
                {
                    foreach (var tag in post.Tags)
                    {
                        Console.WriteLine(tag.Name);
                    }
                }
            }
        }

        private static void SimpleCrudTestsAssociationEntity()
        {
            using (var conn = OpenConnection())
            {
                var db = new DbImpl(conn);

                var post = new Post { CreateDate = DateTime.Now, Title = "Test Post" };
                var user = new User { JoinDate = DateTime.Now, Username = "Test User" };

                db.Insert(post);
                db.Insert(user);

                var like = new Like
                {
                    Post = post,
                    User = user,
                    LikedDate = DateTime.Now
                };

                db.Insert(like);

                like = db.GetById<Like>(like);
                Console.WriteLine(like.LikedDate);

                like.LikedDate = DateTime.Now.AddDays(2);
                db.Update(like);

                like = db.GetById<Like>(like);
                Console.WriteLine(like.LikedDate);

                db.Delete(like);

                like = db.GetById<Like>(like);
                Console.WriteLine(like == null);
            }
        }

        private static void SimpleCrudTestsEntity()
        {
            using (var conn = OpenConnection())
            {
                var db = new DbImpl(conn);

                var user = new User
                {
                    Username = "testuser1",
                    Fullname = "Test User1",
                    FacebookId = "23345",
                    Email = "testuser1@mail.com",
                    JoinDate = DateTime.Now,
                    Status = UserStatus.AwaitingActivation,
                    Type = UserType.Admin,
                    Password = "1234"
                };

                db.Insert(user);

                user = db.GetById<User>(user.Id);
                Console.WriteLine(user.Username);

                user.Username = "testuser" + DateTime.Now.Ticks;
                db.Update(user);

                user = db.GetById<User>(user.Id);
                Console.WriteLine(user.Username);

                db.Delete(user);

                user = db.GetById<User>(user.Id);
                Console.WriteLine(user == null);
            }
        }

        private static void SelectTests()
        {
            var userIds = new long[] { 1, 2, 3 };
            var post = new Post { Id = 12 };

            using (var conn = OpenConnection())
            {
                var db = new DbImpl(conn);

                Page<Like> page = null;

                var sw = new Stopwatch();
                sw.Start();
                for (var i = 0; i < 100; i++)
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
        }

        private static IDbConnection OpenConnection()
        {
            var conn = new SqlConnection("Server=.;Database=TagKid2;uid=sa;pwd=123456");
            conn.Open();
            return conn;
        }
    }
}
