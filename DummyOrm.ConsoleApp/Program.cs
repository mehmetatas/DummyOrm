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
                .Register<Post>()
                .Register<Like>();

            using (_repo = CreateRepo())
            {
                //Insert();
                //Update();
                //Delete();
                //SelectById();
                //Select();
                //InsertNullable();
                //SelectNullable();
                //WhereIsNull();
                //InsertSelectByteArray();
                //InsertNullByteArray();
                //Top();
                //Page();
                Include();
                //Tuple();
                //PageTuple();
                //TopTuple();
                //GetById();
                //InsertAssociationEntity();
                //GetByIdAssociationEntity();
                //UpdateAssociationEntity();
                //DeleteAssociationEntity();
            }

            Console.WriteLine("OK!");
            Console.ReadLine();
        }

        private static void DeleteAssociationEntity()
        {
            var like = new Like
            {
                PostId = 1,
                UserId = 2,
                LikedDate = DateTime.Now
            };

            _repo.Insert(like);

            like = _repo.GetById<Like>(new Like { PostId = 1, UserId = 2 });
            _repo.Delete(like);
            like = _repo.GetById<Like>(new Like { PostId = 1, UserId = 2 });
            Console.WriteLine(like == null);
        }

        private static void UpdateAssociationEntity()
        {
            var like = _repo.GetById<Like>(new Like { PostId = 1, UserId = 1 });
            var time = DateTime.Now;
            like.LikedDate = DateTime.Now;
            _repo.Update(like);
            like = _repo.GetById<Like>(new Like { PostId = 1, UserId = 1 });

            Console.WriteLine(time);
            Console.WriteLine(like.LikedDate);
        }

        private static void GetByIdAssociationEntity()
        {
            var like = _repo.GetById<Like>(new Like { PostId = 1, UserId = 1 });
            Console.WriteLine(like.PostId);
            Console.WriteLine(like.UserId);
        }

        private static void InsertAssociationEntity()
        {
            var like = new Like
            {
                PostId = 1,
                UserId = 1,
                LikedDate = DateTime.Now
            };

            _repo.Insert(like);
        }

        private static void GetById()
        {
            var post = _repo.GetById<Post>(1);
            Console.WriteLine(post.Id);
        }

        private static void TopTuple()
        {
            var tuplePage = _repo.Select<Post>()
                .Join<User>((user, post) => user.Id == post.User.Id)
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
                .Join<User>((user, post) => user.Id == post.User.Id)
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
                .LeftJoin<User>((user, post) => user.Id == post.User.Id)
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
                .Include(p => p.Id, p => p.Title, p => p.User)
                .ReadFirst();

            Console.WriteLine(post.Id);
            Console.WriteLine(post.HtmlContent);
            Console.WriteLine(post.Title);
            Console.WriteLine(post.User != null);
            Console.WriteLine(post.User.Id);
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
            Console.WriteLine(posts.Items[0].User.Id);
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
                User = new User { Id = 1 },
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

        private static void InsertNullByteArray()
        {
            var post = new Post
            {
                User = new User { Id = 1 },
                CreateDate = DateTime.Now,
                Data = null
            };

            _repo.Insert(post);
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
                User = new User { Id = 1 },
                UpdateDate = DateTime.Now,
                AccessLevel = AccessLevel.Public,
                Title = "Title",
                CreateDate = DateTime.Now,
                Data = new byte[] { 1 }
            });
        }

        private static void Select()
        {
            var post = _repo.Select<Post>()
                .Where(p => p.Id == 2)
                .ReadFirst();

            Console.WriteLine(post.User.Id);
        }

        private static void SelectById()
        {
            var post = _repo.GetById<Post>(2);

            Console.WriteLine(post.User.Id);
        }

        private static void Delete()
        {
            _repo.Delete(new User { Id = 3 });
        }

        private static void Update()
        {
            var post = new Post
            {
                Id = 2,
                CreateDate = DateTime.Now,
                User = new User
                {
                    Id = 4
                }
            };

            _repo.Update(post);
        }

        private static void Insert()
        {
            _repo.Insert(new Post
            {
                CreateDate = DateTime.Now,
                User = new User { Id = 3 }
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