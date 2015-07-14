using System.Linq.Expressions;
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

            ExpressionTest<User>(u => new { u.Id, u.Username });

            using (_repo = CreateRepo())
            {
                // SimpleSelect();
                // SelectReference();
                //SimpleSelectWhere();
                //SelectWhereReference();
            }

            Console.WriteLine("OK!");
            Console.ReadLine();
        }

        private static void ExpressionTest<T>(Expression<Func<T, object>> exp)
        {
            var args = (exp.Body as NewExpression).Arguments;
            foreach (var arg in args)
            {
                Console.WriteLine((arg as MemberExpression).Member.Name);
            }
        }

        private static void SelectWhereReference()
        {
            _repo.Select<Like>()
                .Join(l => l.Post)
                .Where(l => l.Post.User.Id == 3)
                .ToList();
        }

        private static void SimpleSelectWhere()
        {
            var posts = _repo.Select<Post>()
                .Where(p => p.Title.Contains("it"))
                .ToList();

            foreach (var post in posts)
            {
                Console.WriteLine(post.Title);
                Console.WriteLine(post.User.Id);
            }
        }

        private static void SelectReference()
        {
            var likes = _repo.Select<Like>()
                .Join(l => l.User)
                .Join(l => l.Post.User)
                .Join(l => l.Post)
                .ToList();

            foreach (var like in likes)
            {
                Console.WriteLine("{0} liked {1} by {2} at {3}", like.User.Username, like.Post.Title, like.Post.User.Username, like.LikedDate);
            }
        }

        private static void SimpleSelect()
        {
            var posts = _repo.Select<Post>()
                .ToList();

            foreach (var post in posts)
            {
                Console.WriteLine(post.Title);
                Console.WriteLine(post.User.Id);
            }
        }

        private static IRepository CreateRepo()
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            conn.Open();
            return new Repository.Repository(conn);
        }
    }
}