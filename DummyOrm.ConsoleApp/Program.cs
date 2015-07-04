using DummyOrm.ConsoleApp.Entities;
using DummyOrm.Meta;
using DummyOrm.Repository;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DummyOrm.ConsoleApp
{
    class Program
    {
        private static IRepository _repo;

        static void Main(string[] args)
        {
            DbMeta.Instance.Register<User>()
                .Register<Post>();

            using (_repo = CreateRepo())
            {
                //Insert();
                //Update();
                //Delete();
                //SelectById();
                //InsertNullable();
                //SelectNullable();
                //WhereIsNull();
                //InsertSelectByteArray();
                //Top();
                //Page();
                //Include();
                //Tuple();
                //PageTuple();
                TopTuple();
            }

            Console.WriteLine("OK!");
            Console.ReadLine();
        }

        private static void TopTuple()
        {
            var tuplePage = _repo.Select<Post>()
                .Join<User>((user, post) => user.Id == post.UserId)
                .Include<User>(u => u.Username)
                .Include(p => p.Title)
                .Where<User>(u => u.Id == 1)
                .Top<Post, User>(2);

            foreach (var tuple in tuplePage.Items)
            {
                Console.WriteLine("{0} by {1}", tuple.Item1.Title,
                    tuple.Item2 == null
                        ? "null"
                        : tuple.Item2.Username);
            }
        }

        private static void PageTuple()
        {
            var tuplePage = _repo.Select<Post>()
                .Join<User>((user, post) => user.Id == post.UserId)
                .Include<User>(u => u.Username)
                .Include(p => p.Title)
                .Where<User>(u => u.Id == 1)
                .Page<Post, User>(1, 2);

            foreach (var tuple in tuplePage.Items)
            {
                Console.WriteLine("{0} by {1}", tuple.Item1.Title,
                    tuple.Item2 == null
                        ? "null"
                        : tuple.Item2.Username);
            }
        }

        private static void Tuple()
        {
            var tuples = _repo.Select<Post>()
                .LeftJoin<User>((user, post) => user.Id == post.UserId)
                .Include<User>()
                .Exclude(p => p.Data)
                .Read<Post, User>();
            
            foreach (var tuple in tuples)
            {
                Console.WriteLine("{0} by {1}", tuple.Item1.Title,
                    tuple.Item2 == null
                        ? "null"
                        : tuple.Item2.Username);
            }
        }

        private static void Include()
        {
            var post = _repo.Select<Post>()
                .Include(p => p.Id, p => p.Title)
                .ReadFirst();

            Console.WriteLine(post.Id);
            Console.WriteLine(post.HtmlContent);
            Console.WriteLine(post.Title);
        }

        private static void Page()
        {
            var posts = _repo.Select<Post>()
                .Page(2, 1);

            Console.WriteLine(posts.PageIndex);
            Console.WriteLine(posts.PageSize);
            Console.WriteLine(posts.TotalCount);
            Console.WriteLine(posts.PageCount);
            Console.WriteLine(posts.Items.Length);
            Console.WriteLine(posts.Items[0].Id);
        }

        private static void Top()
        {
            var posts = _repo.Select<Post>()
                .Top(2);

            Console.WriteLine(posts.TotalCount);
            Console.WriteLine(posts.PageCount);
            Console.WriteLine(posts.PageIndex);
            Console.WriteLine(posts.PageSize);
            Console.WriteLine(posts.HasMore);
        }

        private static void InsertSelectByteArray()
        {
            var post = new Post
            {
                UserId = 1,
                CreateDate = DateTime.Now,
                Data = new byte[] { 1, 2, 3, 4 }
            };

            _repo.Insert(post);

            var id = post.Id;

            post = _repo.Select<Post>()
                .Where(p => p.Id == id)
                .ReadFirst();

            Console.WriteLine(post.Data.Length);
        }

        private static void WhereIsNull()
        {
            var posts = _repo.Select<Post>()
                  .Where(p => p.UpdateDate != null && p.AccessLevel == AccessLevel.Private)
                  .Read();

            foreach (var post in posts)
            {
                Console.WriteLine(post.UpdateDate == null);
            }
        }

        private static void SelectNullable()
        {
            var posts = _repo.Select<Post>()
                .OrderByDesc(p => p.Id)
                .Read();

            foreach (var post in posts)
            {
                Console.WriteLine(post.UpdateDate == null);
            }
        }

        private static void InsertNullable()
        {
            _repo.Insert(new Post
            {
                UserId = 1,
                UpdateDate = DateTime.Now,
                AccessLevel = AccessLevel.Public,
                Title = "Title",
                CreateDate = DateTime.Now
            });
        }

        private static void SelectById()
        {
            var user = _repo.Select<User>()
                .Where(u => u.Id == 2)
                .ReadFirst();

            Console.WriteLine(user.Username);
        }

        private static void Delete()
        {
            _repo.Delete(new User { Id = 3 });
        }

        private static void Update()
        {
            var user = new User
            {
                Id = 3,
                Username = "taga UPDATED",
                Email = "taga@mail.com",
                Type = UserType.Admin,
                Status = UserStatus.AwaitingActivation,
                JoinDate = DateTime.Now,
                Fullname = "Taga",
                FacebookId = "987654321",
                Password = "123456"
            };

            _repo.Update(user);
        }

        private static void Insert()
        {
            _repo.Insert(new User
            {
                Username = "taga",
                Email = "taga@mail.com",
                Type = UserType.Admin,
                Status = UserStatus.AwaitingActivation,
                JoinDate = DateTime.Now,
                Fullname = "Taga",
                FacebookId = "987654321",
                Password = "123456"
            });
        }

        private static IRepository CreateRepo()
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            conn.Open();
            return new Repository.Repository(conn);
        }
    }
}