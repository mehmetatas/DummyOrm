﻿using System.Linq;
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

            var query = new QueryImpl<Like>();

            var list = query
                .Join(l => l.User)
                .Join(l => l.Post)
                .Join(l => l.Post.User)
                .Where(l => DateTime.Now.ToUniversalTime().ToLocalTime().ToUniversalTime() > l.User.JoinDate && userIds.Contains(l.User.Id) && l.Post.User.Username.StartsWith("taga".Substring(0, 2)) && l.Post.User.Id != l.User.Id)
                .ToList();

            Console.WriteLine(list.Count);

            Console.ReadLine();
        }
    }
}
