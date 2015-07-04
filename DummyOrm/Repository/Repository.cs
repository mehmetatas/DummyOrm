using System;
using DummyOrm.CommandBuilders;
using DummyOrm.Meta;
using DummyOrm.QueryBuilders.Select;
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
                ExecuteNonQuery(entity, CommandBuilder.Insert);
            }
            else
            {
                var res = ExecuteScalar(entity, CommandBuilder.Insert);
                var idProp = tableMeta.IdColumn.Property;
                idProp.SetValue(entity, Convert.ChangeType(res, idProp.PropertyType));
            }
        }

        public int Update(object entity)
        {
            return ExecuteNonQuery(entity, CommandBuilder.Update);
        }

        public int Delete(object entity)
        {
            return ExecuteNonQuery(entity, CommandBuilder.Delete);
        }

        public IQuery<T> Select<T>()
        {
            return new RepositoryQuery<T>(this, new QueryBuilder<T>());
        } 

        private int ExecuteNonQuery(object entity, CommandBuilder cmdBuilder)
        {
            var cmd = BuildCommand(entity, cmdBuilder);
            return cmd.ExecuteNonQuery();
        }

        private object ExecuteScalar(object entity, CommandBuilder cmdBuilder)
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

        private IDbCommand BuildCommand(object entity, CommandBuilder cmdBuilder)
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
                dbParam.Value = parameter.Value;
                dbCmd.Parameters.Add(dbParam);

                Console.WriteLine("{0}: {1}", parameter.Key, parameter.Value);
            }

            return dbCmd;
        }

        public void Dispose()
        {
            _conn.Dispose();
        }
    }
}
