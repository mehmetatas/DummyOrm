using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using DummyOrm.Sql;

namespace DummyOrm.Db
{
    public interface IDb : IDisposable
    {
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        void Rollback();

        void Commit();

        void Insert<T>(T entity) where T : class, new();

        void Update<T>(T entity) where T : class, new();

        void Delete<T>(T entity) where T : class, new();

        T GetById<T>(object id) where T : class, new();

        IQuery<T> Select<T>() where T : class, new();

        IList<T> Select<T>(Command selectCommand) where T : class, new();

        void LoadMany<T, TProp>(IList<T> entities, Expression<Func<T, IList<TProp>>> listExp, Expression<Func<TProp, object>> includeProps = null)
            where T : class, new()
            where TProp : class, new();

        void Load<T, TProp>(IList<T> entities, Expression<Func<T, TProp>> propExp, Expression<Func<TProp, object>> includeProps = null)
            where T : class, new()
            where TProp : class, new();
    }

    public static class DbExtensions
    {
        public static void LoadMany<T, TProp>(this IDb db, T entity, Expression<Func<T, IList<TProp>>> listExp, Expression<Func<TProp, object>> includeProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            db.LoadMany(new[] { entity }, listExp, includeProps);
        }

        public static void Load<T, TProp>(this IDb db, T entity, Expression<Func<T, TProp>> propExp, Expression<Func<TProp, object>> includeProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            db.Load(new[] { entity }, propExp, includeProps);
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