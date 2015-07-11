using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DummyOrm.ConsoleApp.Entities;
using DummyOrm.Execution;
using DummyOrm.Meta;
using DummyOrm.Repository;
using System;
using System.Configuration;
using System.Data.SqlClient;
using DummyOrm.Sql;
using DummyOrm.Sql.QueryBuilders.Select;

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
                // SimpleSelect();
                // SelectReference();
                SimpleSelectWhere();
            }

            Console.WriteLine("OK!");
            Console.ReadLine();
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