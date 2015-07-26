using DummyOrm.ConsoleApp.Entities;
using DummyOrm.ConsoleApp.Models;
using DummyOrm.Db;
using DummyOrm.Meta;
using DummyOrm.Providers.SqlServer2012;
using DummyOrm.Sql;
using DummyOrm.Sql.Command;
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
        static void Main()
        {
            Init();

            Readme();

            JoinTest();
            SelectWall();
            //SelectModel();
            SimpleCrudTestsAssociationEntity();
            SimpleCrudTestsEntity();
            SelectTests();
            SelectOneToOne();
            SelectOneToMany();
            SelectManyToMany();
            DeleteMany();

            Console.WriteLine("OK!");
            Console.ReadLine();
        }

        private static void DeleteMany()
        {
            using (var db = OpenConnection())
            {
                db.BeginTransaction();
                db.DeleteMany<Like>(l => l.LikedDate == DateTime.Now);
            }
        }

        private static void Readme()
        {
            using (var db = OpenConnection())
            {
                var users = db.Select<User>().ToList();

                db.LoadMany(users, u => u.Posts, p => new { p.User.Username, p.Title, p.PublishDate });
            }
        }

        private static void Init()
        {
            var builder = Db.Db.Setup(new TestDbProvider());

            builder.Table<Post>()
                .Table<User>()
                .Table<Like>()
                .Table<Tag>()
                .Table<PostTag>();

            builder
                .OneToMany<User, Post>(u => u.Posts, p => p.User)
                .ManyToMany<Post, PostTag>(p => p.Tags);
        }

        private static void JoinTest()
        {
            using (var db = OpenConnection())
            {
                db.Select<Like>()
                    .Where(l => l.Post.User.Id == 3)
                    .ToList();
            }
        }

        private static void SelectWall()
        {
            var loggedInUserId = 1;
            var lastFetchedPostId = 5;

            using (var db = OpenConnection())
            {
                var followedUserIds = db.Select<FollowUser>()
                    .Where(fu => fu.FollowerUser.Id == loggedInUserId)
                    .ToList()
                    .Select(fu => fu.FollowedUser.Id);

                var posts = db.Select<Post>()
                    .Join(p => p.User, u => new { u.Id, u.Username })
                    .Include(p => new { p.Id, p.Title })
                    .Where(p => followedUserIds.Contains(p.Id) && p.Id > lastFetchedPostId)
                    .Top(5);

                db.LoadMany(posts.Items, p => p.Tags);
            }
        }

        private static void SelectModel()
        {
            var cmd = new CommandBuilder().Append(@"select 
	p.Id PostId, 
	p.UserId UserId,
	u.Username Username,
	p.Title PostTitle,
	count(pt.TagId) TagCount,
	count(pl.PostId) LikeCount
from Post p
join [User] u on u.Id = p.UserId 
left join PostTag pt on pt.PostId = p.Id
left join [Like] pl on pl.PostId = p.Id
where
    p.CreateDate < @pCreateDate
group by
	p.Id, p.UserId, u.Username, p.Title")
                .AddParameter("pCreateDate", DateTime.Now)
                .Build();

            using (var db = OpenConnection())
            {
                var list = db.ExecuteQuery<PostListModel>(cmd);

                foreach (var post in list)
                {
                    Console.WriteLine("{0}: {1} [{2} Likes] [{3} Tags] {4}", post.Username, post.PostTitle, post.LikeCount, post.TagCount, post.PostId);
                }
            }
        }

        private static void SelectOneToOne()
        {
            using (var db = OpenConnection())
            {
                var posts = db.Select<Post>().ToList();

                db.Load(posts, p => p.User, u => new { u.Username, u.Fullname });
            }
        }

        private static void SelectOneToMany()
        {
            using (var db = OpenConnection())
            {
                var users = db.Select<User>().ToList();

                db.LoadMany(users, u => u.Posts, p => new { p.User.Username, p.Title, p.PublishDate });
            }
        }

        private static void SelectManyToMany()
        {
            using (var db = OpenConnection())
            {
                var posts = db.Select<Post>().ToList();

                db.LoadMany(posts, p => p.Tags, t => new { t.Name });
            }
        }

        private static void SimpleCrudTestsAssociationEntity()
        {
            using (var db = OpenConnection())
            {
                var post = new Post { CreateDate = DateTime.Now, Title = "Test Post", User = new User { Id = 4 } };
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
            using (var db = OpenConnection())
            {
                var user = new User
                {
                    Username = "testuser1",
                    Fullname = "Test User1",
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

            using (var db = OpenConnection())
            {
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

        private static IDb OpenConnection()
        {
            return Db.Db.Create();
        }
    }

    public class TestDbProvider : SqlServer2012Provider
    {
        public override IDbConnection CreateConnection()
        {
            return new SqlConnection("Server=.;Database=DummyOrmTest;uid=sa;pwd=123456");
        }
    }
}
