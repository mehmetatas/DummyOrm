using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using DummyOrm.Dynamix.Impl;
using DummyOrm.Meta;
using DummyOrm.Sql;
using DummyOrm.Sql.SimpleCommands;

namespace DummyOrm.Db.Impl
{
    class DbImpl : IDb, ICommandExecutor
    {
        private IDbTransaction _tran;
        private readonly IDbConnection _conn;
        private readonly ICommandExecutor _cmdExec;

        public DbImpl(IDbConnection conn)
        {
            _conn = conn;
            _cmdExec = this;
        }

        public void Insert<T>(T entity) where T : class, new()
        {
            var cmd = SimpleCommandBuilder.Insert.Build(entity);
            var id = _cmdExec.ExecuteScalar(cmd);

            if (id == null)
            {
                return;
            }

            var tableMeta = DbMeta.Instance.GetTable<T>();
            tableMeta.IdColumn.GetterSetter.Set(entity, id);
        }

        public void Update<T>(T entity) where T : class, new()
        {
            var cmd = SimpleCommandBuilder.Update.Build(entity);
            _cmdExec.ExecuteNonQuery(cmd);
        }

        public void Delete<T>(T entity) where T : class, new()
        {
            var cmd = SimpleCommandBuilder.Delete.Build(entity);
            _cmdExec.ExecuteNonQuery(cmd);
        }

        public T GetById<T>(object id) where T : class, new()
        {
            var cmd = SimpleCommandBuilder.Select.BuildById<T>(id);
            using (var reader = _cmdExec.ExecuteReader(cmd))
            {
                if (!reader.Read())
                {
                    return null;
                }
                var deserializer = PocoDeserializer.GetDefault<T>();
                return deserializer.Deserialize(reader) as T;
            }
        }

        public IQuery<T> Select<T>() where T : class, new()
        {
            return new QueryImpl<T>(this);
        }

        public IList<T> Select<T>(Command selectCommand) where T : class, new()
        {
            using (var reader = _cmdExec.ExecuteReader(selectCommand))
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

        public void LoadMany<T, TProp>(IList<T> entities, Expression<Func<T, IList<TProp>>> listExp, Expression<Func<TProp, object>> includeProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            var assoc = DbMeta.Instance.GetAssociation(listExp);
            assoc.Loader.Load(entities, this, includeProps);
        }

        public void Load<T, TProp>(IList<T> entities, Expression<Func<T, TProp>> propExp, Expression<Func<TProp, object>> includeProps = null) where T : class, new() where TProp : class, new()
        {
            var colMeta = DbMeta.Instance.GetColumn(propExp);
            colMeta.Loader.Load(entities, this, includeProps);
        }

        private IDbCommand CreateCommand(Command cmd)
        {
            var dbCmd = _conn.CreateCommand();

            dbCmd.CommandText = cmd.CommandText;

            foreach (var sqlParameter in cmd.Parameters)
            {
                var param = dbCmd.CreateParameter();

                param.ParameterName = sqlParameter.Key;
                param.Value = sqlParameter.Value.Value ?? DBNull.Value;
                param.DbType = sqlParameter.Value.ColumnMeta.DbType;
                param.Precision = sqlParameter.Value.ColumnMeta.DecimalPrecision;
                param.Size = sqlParameter.Value.ColumnMeta.StringLength;

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

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_tran != null)
            {
                _tran = _conn.BeginTransaction(isolationLevel);
            }
        }

        public void Commit()
        {
            if (_tran != null)
            {
                _tran.Commit();
            }
        }

        public void Rollback()
        {
            if (_tran != null)
            {
                _tran.Rollback();
            }
        }

        public void Dispose()
        {
            _conn.Dispose();
            if (_tran != null)
            {
                _tran.Dispose();
            }
        }

        IDataReader ICommandExecutor.ExecuteReader(Command cmd)
        {
            using (var dbCmd = CreateCommand(cmd))
            {
                return dbCmd.ExecuteReader();
            }
        }

        int ICommandExecutor.ExecuteNonQuery(Command cmd)
        {
            using (var dbCmd = CreateCommand(cmd))
            {
                return dbCmd.ExecuteNonQuery();
            }
        }

        object ICommandExecutor.ExecuteScalar(Command cmd)
        {
            using (var dbCmd = CreateCommand(cmd))
            {
                return dbCmd.ExecuteScalar();
            }
        }
    }
}