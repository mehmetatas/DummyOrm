using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using DummyOrm.Dynamix.Impl;
using DummyOrm.Meta;
using DummyOrm.Sql.Command;

namespace DummyOrm.Db.Impl
{
    public class DbImpl : IDb, ICommandExecutor
    {
        private IDbTransaction _tran;
        private readonly IDbConnection _conn;

        protected internal DbImpl()
        {
            _conn = DbMeta.Instance.DbProvider.CreateConnection();
            _conn.Open();
        }

        public virtual void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_tran == null)
            {
                _tran = _conn.BeginTransaction(isolationLevel);
            }
        }

        public virtual void Commit()
        {
            if (_tran != null)
            {
                _tran.Commit();
            }
        }

        public virtual void Rollback()
        {
            if (_tran != null)
            {
                _tran.Rollback();
            }
        }

        public virtual void Insert<T>(T entity) where T : class, new()
        {
            var cmd = SimpleCommandBuilder.Insert.Build(entity);
            var id = ExecuteScalar(cmd);

            if (id == null)
            {
                return;
            }

            var tableMeta = DbMeta.Instance.GetTable<T>();
            tableMeta.IdColumn.GetterSetter.Set(entity, id);
        }

        public virtual void Update<T>(T entity) where T : class, new()
        {
            var cmd = SimpleCommandBuilder.Update.Build(entity);
            ExecuteNonQuery(cmd);
        }

        public virtual void Delete<T>(T entity) where T : class, new()
        {
            var cmd = SimpleCommandBuilder.Delete.Build(entity);
            ExecuteNonQuery(cmd);
        }

        public virtual T GetById<T>(object id) where T : class, new()
        {
            var cmd = SimpleCommandBuilder.Select.BuildById<T>(id);
            using (var reader = ExecuteReaderInternal(cmd))
            {
                if (!reader.Read())
                {
                    return null;
                }
                var deserializer = PocoDeserializer.GetDefault<T>();
                return deserializer.Deserialize(reader) as T;
            }
        }

        public virtual IQuery<T> Select<T>() where T : class, new()
        {
            return new QueryImpl<T>(this);
        }

        public virtual IList<T> ExecuteQuery<T>(Command selectCommand) where T : class, new()
        {
            using (var reader = ExecuteReaderInternal(selectCommand))
            {
                var deserializer = PocoDeserializer.GetDefault<T>();

                var list = new List<T>();

                while (reader.Read())
                {
                    var entity = (T)deserializer.Deserialize(reader);
                    list.Add(entity);
                }

                return list;
            }
        }

        public virtual int ExecuteNonQuery(Command cmd)
        {
            using (var dbCmd = CreateCommand(cmd))
            {
                return dbCmd.ExecuteNonQuery();
            }
        }

        public virtual object ExecuteScalar(Command cmd)
        {
            using (var dbCmd = CreateCommand(cmd))
            {
                return dbCmd.ExecuteScalar();
            }
        }

        public virtual void Load<T, TProp>(IList<T> entities, Expression<Func<T, TProp>> propExp, Expression<Func<TProp, object>> includeProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            var colMeta = DbMeta.Instance.GetColumn(propExp);
            colMeta.Loader.Load(entities, this, includeProps);
        }

        public virtual void LoadMany<T, TProp>(IList<T> entities, Expression<Func<T, IList<TProp>>> listExp, Expression<Func<TProp, object>> includeProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            var assoc = DbMeta.Instance.GetAssociation(listExp);
            assoc.Loader.Load(entities, this, includeProps);
        }

        public virtual void Dispose()
        {
            _conn.Dispose();
            if (_tran != null)
            {
                _tran.Dispose();
            }
        }

        IDataReader ICommandExecutor.ExecuteReader(Command cmd)
        {
            return ExecuteReaderInternal(cmd);
        }

        int ICommandExecutor.ExecuteNonQuery(Command cmd)
        {
            return ExecuteNonQuery(cmd);
        }

        object ICommandExecutor.ExecuteScalar(Command cmd)
        {
            return ExecuteScalar(cmd);
        }

        private IDbCommand CreateCommand(Command cmd)
        {
            var dbCmd = _conn.CreateCommand();

            dbCmd.CommandText = cmd.CommandText;
            dbCmd.CommandType = cmd.Type;
            
            foreach (var sqlParameter in cmd.Parameters)
            {
                var param = dbCmd.CreateParameter();

                var paramMeta = sqlParameter.Value.ParameterMeta;

                param.ParameterName = sqlParameter.Key;
                param.Value = sqlParameter.Value.Value ?? DBNull.Value;

                param.DbType = paramMeta.DbType;
                param.Precision = paramMeta.DecimalPrecision;
                param.Size = paramMeta.StringLength;

                dbCmd.Parameters.Add(param);
            }
#if DEBUG
            Console.WriteLine(dbCmd.CommandText);
            //foreach (var param in dbCmd.Parameters.Cast<IDbDataParameter>())
            //{
            //    Console.WriteLine("{0}: {1}", param.ParameterName, param.Value);
            //}
#endif
            return dbCmd;
        }

        private IDataReader ExecuteReaderInternal(Command cmd)
        {
            using (var dbCmd = CreateCommand(cmd))
            {
                return dbCmd.ExecuteReader();
            }
        }
    }
}