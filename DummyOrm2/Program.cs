using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using DummyOrm2.Entities;
using DummyOrm2.Orm;

namespace DummyOrm2
{
    class Program
    {
        static void Main(string[] args)
        {
            TestJoinKey<Like, string>(l => l.Post.User.Username);
            Console.ReadLine();
        }

        private static void TestJoinKey<T, TProp>(Expression<Func<T, TProp>> exp)
        {
            Console.WriteLine(exp.GetJoinKey());
        }
    }
}
