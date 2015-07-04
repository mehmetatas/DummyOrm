using System;
using System.Linq;
using DummyOrm.Meta;
using DummyOrm.Repository.PocoMappers;
using DummyOrm.Sql.QueryBuilders;
using DummyOrm.Sql.QueryBuilders.Select;
using DummyOrm.Sql;
using System.Data;

namespace DummyOrm.Repository
{
    public class Repository : IRepository, ISelectQueryReader
    {
        private readonly IDbConnection _conn;

        public Repository(IDbConnection conn)
        {
            _conn = conn;
        }

        public void Insert(object entity)
        {
            var tableMeta = DbMeta.Instance.GetTable(entity.GetType());

            if (tableMeta.AssociationTable)
            {
                ExecuteNonQuery(entity, SimpleCommandBuilder.Insert);
            }
            else
            {
                var res = ExecuteScalar(entity, SimpleCommandBuilder.Insert);
                var idProp = tableMeta.IdColumn.Property;
                idProp.SetValue(entity, Convert.ChangeType(res, idProp.PropertyType));
            }
        }

        public int Update(object entity)
        {
            return ExecuteNonQuery(entity, SimpleCommandBuilder.Update);
        }

        public int Delete(object entity)
        {
            return ExecuteNonQuery(entity, SimpleCommandBuilder.Delete);
        }

        public T GetById<T>(object id)
        {
            var cmd = id.GetType() == typeof(T)
                ? SimpleCommandBuilder.Select.Build(id)
                : SimpleCommandBuilder.Select.BuildById<T>(id);

            var tableMapping = DbMeta.Instance.GetTable<T>();
            var outputMappings = tableMapping.Columns.ToDictionary(c => c.Column.Alias, c => c);

            var dbCmd = BuildCommand(cmd);
            var mapper = new SinglePocoMapper(outputMappings);
            var pocoReader = new PocoReader<T>(dbCmd.ExecuteReader(), mapper);
            return pocoReader.FirstOrDefault();
        }

        public IQuery<T> Select<T>()
        {
            return new RepositoryQuery<T>(this, new SelectQueryBuilder<T>());
        } 

        private int ExecuteNonQuery(object entity, ISimpleCommandBuilder cmdBuilder)
        {
            var cmd = BuildCommand(entity, cmdBuilder);
            return cmd.ExecuteNonQuery();
        }

        private object ExecuteScalar(object entity, ISimpleCommandBuilder cmdBuilder)
        {
            var cmd = BuildCommand(entity, cmdBuilder);
            return cmd.ExecuteScalar();
        }

        IDataReader ISelectQueryReader.ExecuteReader(SelectQuery query)
        {
            var cmd = query.ToSqlCommand();

            var dbCmd = BuildCommand(cmd);

            return dbCmd.ExecuteReader();
        }

        private IDbCommand BuildCommand(object entity, ISimpleCommandBuilder cmdBuilder)
        {
            var cmd = cmdBuilder.Build(entity);

            return BuildCommand(cmd);
        }

        private IDbCommand BuildCommand(SqlCommand cmd)
        {
            var dbCmd = _conn.CreateCommand();
            dbCmd.CommandText = cmd.CommandText;

            Console.WriteLine(cmd.CommandText);

            foreach (var parameter in cmd.Parameters)
            {
                var dbParam = dbCmd.CreateParameter();
                dbParam.ParameterName = parameter.Key;
                dbParam.Value = parameter.Value.Value;
                dbParam.DbType = parameter.Value.DbType;
                dbCmd.Parameters.Add(dbParam);

                Console.WriteLine("{0}: {1}", parameter.Key, parameter.Value.Value);
            }

            return dbCmd;
        }

        public void Dispose()
        {
            _conn.Dispose();
        }
    }
}
