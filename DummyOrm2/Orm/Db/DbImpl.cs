using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using DummyOrm2.Orm.Sql;
using DummyOrm2.Orm.Sql.Select;

namespace DummyOrm2.Orm.Db
{
    public class DbImpl : IDb, IQueryExecuter
    {
        private readonly IDbConnection _conn;

        public DbImpl(IDbConnection conn)
        {
            _conn = conn;
        }

        public void Insert<T>(T entity) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T entity) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(T entity) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(Expression<Func<T, bool>> filter) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public T GetById<T>(object id) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IQuery<T> Select<T>() where T : class, new()
        {
            return new QueryImpl<T>(this);
        }

        public IFillQuery<T> Select<T>(Expression<Func<T, IList>> list) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public T CreateProxy<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        IDataReader IQueryExecuter.Execute<T>(SelectQuery<T> query)
        {
            var builder = new SqlServerSelectSqlCommandBuilderImpl();
            var sqlCmd = builder.Build(query);

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = sqlCmd.CommandText;

                foreach (var sqlParameter in sqlCmd.Parameters)
                {
                    var param = cmd.CreateParameter();

                    param.ParameterName = sqlParameter.Key;
                    param.Value = sqlParameter.Value.Value ?? DBNull.Value;

                    // TODO: Fix parameter properties
                    //param.DbType = sqlParameter.Value.ColumnMeta.DbType;
                    //param.Precision = sqlParameter.Value.ColumnMeta.DecimalPrecision;
                    //param.Size = sqlParameter.Value.ColumnMeta.StringLength;

                    cmd.Parameters.Add(param);
                }

#if DEBUG
                Console.WriteLine(cmd.CommandText);
                foreach (var param in cmd.Parameters.Cast<IDbDataParameter>())
                {
                    Console.WriteLine("{0}: {1}", param.ParameterName, param.Value);
                }
#endif
                return cmd.ExecuteReader();
            }
        }
    }
}