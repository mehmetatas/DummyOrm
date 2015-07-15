using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using DummyOrm.Dynamix.Impl;
using DummyOrm.Meta;
using DummyOrm.Sql;
using DummyOrm.Sql.SimpleCommands;

namespace DummyOrm.Db.Impl
{
    public class DbImpl : IDb, ICommandExecutor
    {
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

        public void Load<T>(IList<T> entities, Expression<Func<T, IList>> listExp) where T : class, new()
        {
            var assoc = DbMeta.Instance.GetAssociation(listExp);
            assoc.Loader.Load(entities, this);
        }

        private IDbCommand CreateCommand(Command sqlCmd)
        {
            var cmd = _conn.CreateCommand();

            cmd.CommandText = sqlCmd.CommandText;

            foreach (var sqlParameter in sqlCmd.Parameters)
            {
                var param = cmd.CreateParameter();

                param.ParameterName = sqlParameter.Key;
                param.Value = sqlParameter.Value.Value ?? DBNull.Value;
                param.DbType = sqlParameter.Value.ColumnMeta.DbType;
                param.Precision = sqlParameter.Value.ColumnMeta.DecimalPrecision;
                param.Size = sqlParameter.Value.ColumnMeta.StringLength;

                cmd.Parameters.Add(param);
            }
#if DEBUG
            Console.WriteLine(cmd.CommandText);
            foreach (var param in cmd.Parameters.Cast<IDbDataParameter>())
            {
                Console.WriteLine("{0}: {1}", param.ParameterName, param.Value);
            }
#endif
            return cmd;
        }

        IDataReader ICommandExecutor.ExecuteReader(Command sqlCmd)
        {
            using (var cmd = CreateCommand(sqlCmd))
            {
                return cmd.ExecuteReader();
            }
        }

        int ICommandExecutor.ExecuteNonQuery(Command sqlCmd)
        {
            using (var cmd = CreateCommand(sqlCmd))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        object ICommandExecutor.ExecuteScalar(Command sqlCmd)
        {
            using (var cmd = CreateCommand(sqlCmd))
            {
                return cmd.ExecuteScalar();
            }
        }
    }
}