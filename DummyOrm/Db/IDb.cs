using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DummyOrm.Db
{
    public interface IDb
    {
        void Insert<T>(T entity) where T : class, new();

        void Update<T>(T entity) where T : class, new();

        void Delete<T>(T entity) where T : class, new();

        T GetById<T>(object id) where T : class, new();

        IQuery<T> Select<T>() where T : class, new();

        void Load<T>(IList<T> entities, Expression<Func<T, IList>> listExp) where T : class, new();
    }

    public static class DbExtensions
    {
        public static void Load<T>(this IDb db, T entity, Expression<Func<T, IList>> listExp)
            where T : class, new()
        {
            db.Load(new[] { entity }, listExp);
        }

        public static void Insert<T>(this IDb db, params T[] entities) where T : class, new()
        {
            foreach (var entity in entities)
            {
                db.Insert(entity);
            }
        }

        public static void Insert<T>(this IDb db, IEnumerable<T> entities) where T : class, new()
        {
            foreach (var entity in entities)
            {
                db.Insert(entity);
            }
        }

        public static void Update<T>(this IDb db, params T[] entities) where T : class, new()
        {
            foreach (var entity in entities)
            {
                db.Update(entity);
            }
        }

        public static void Update<T>(this IDb db, IEnumerable<T> entities) where T : class, new()
        {
            foreach (var entity in entities)
            {
                db.Update(entity);
            }
        }

        public static void Delete<T>(this IDb db, params T[] entities) where T : class, new()
        {
            foreach (var entity in entities)
            {
                db.Delete(entity);
            }
        }

        public static void Delete<T>(this IDb db, IEnumerable<T> entities) where T : class, new()
        {
            foreach (var entity in entities)
            {
                db.Delete(entity);
            }
        }
    }
}