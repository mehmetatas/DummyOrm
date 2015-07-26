using DummyOrm.Meta;
using DummyOrm.Sql;
using DummyOrm.Sql.Command;
using DummyOrm.Sql.Delete;
using DummyOrm.Sql.Where;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DummyOrm.Providers.SqlServer2012
{
    public class Sql2012DeleteWhereCommandBuilder : IDeleteManyCommandBuilder, IWhereExpressionListener
    {
        private TableMeta _table;

        public Command Build<T>(Expression<Func<T, bool>> filter) where T : class, new()
        {
            _table = DbMeta.Current.GetTable<T>();

            var whereBuilder = DbMeta.Current.DbProvider.CreateWhereCommandBuilder();

            var cmd = whereBuilder.Build(filter, this);

            var cmdBuilder = new CommandBuilder()
                .AppendFormat("DELETE FROM [{0}] WHERE ", _table.TableName)
                .Append(cmd.CommandText);

            foreach (var parameter in cmd.Parameters.Values)
            {
                cmdBuilder.AddParameter(parameter);
            }

            return cmdBuilder.Build();
        }

        public Column RegisterColumn(IList<ColumnMeta> propChain)
        {
            var requiresJoin = (propChain.Count > 2) ||
                               (propChain.Count == 2 && !propChain[1].Identity);

            if (requiresJoin)
            {
                throw new NotSupportedException("Joins are not supported by DeleteMany!");
            }

            var prop = propChain[0];
            
            return new Column
            {
                Meta = prop,
                Table = new Table
                {
                    Meta = _table
                }
            };
        }
    }
}
