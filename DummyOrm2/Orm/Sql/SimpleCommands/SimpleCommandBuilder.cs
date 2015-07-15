using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DummyOrm2.Orm.Meta;

namespace DummyOrm2.Orm.Sql.SimpleCommands
{
    public abstract class SimpleCommandBuilder : ISimpleCommandBuilder
    {
        public static readonly ISimpleCommandBuilder Insert = new InsertCommandBuilder();
        public static readonly ISimpleCommandBuilder Update = new UpdateCommandBuilder();
        public static readonly ISimpleCommandBuilder Delete = new DeleteCommandBuilder();
        public static readonly IReadCommandBuilder Select = new SelectCommandBuilder();

        private readonly Hashtable _commands = new Hashtable();

        private SimpleCommandBuilder()
        {

        }

        public static void RegisterAll(TableMeta tableMeta)
        {
            Insert.Register(tableMeta);
            Update.Register(tableMeta);
            Delete.Register(tableMeta);
            Select.Register(tableMeta);
        }

        public void Register(TableMeta tableMeta)
        {
            var cmdMeta = CreateCommandMeta(tableMeta);
            _commands.Add(tableMeta.Type, cmdMeta);
        }

        public SqlCommand Build(object entity)
        {
            var type = entity.GetType();

            var cmdMeta = (SqlCommandMeta)_commands[type];

            return cmdMeta.CreateCommand(entity);
        }

        protected abstract SqlCommandMeta CreateCommandMeta(TableMeta tableMeta);

        private class InsertCommandBuilder : SimpleCommandBuilder
        {
            protected override SqlCommandMeta CreateCommandMeta(TableMeta tableMeta)
            {
                return SqlCommandMeta.CreateInsertCommandMeta(tableMeta);
            }
        }

        private class UpdateCommandBuilder : SimpleCommandBuilder
        {
            protected override SqlCommandMeta CreateCommandMeta(TableMeta tableMeta)
            {
                return SqlCommandMeta.CreateUpdateCommandMeta(tableMeta);
            }
        }

        private class DeleteCommandBuilder : SimpleCommandBuilder
        {
            protected override SqlCommandMeta CreateCommandMeta(TableMeta tableMeta)
            {
                return SqlCommandMeta.CreateDeleteCommandMeta(tableMeta);
            }
        }

        public class SelectCommandBuilder : SimpleCommandBuilder, IReadCommandBuilder
        {
            protected override SqlCommandMeta CreateCommandMeta(TableMeta tableMeta)
            {
                return SqlCommandMeta.CreateSelectCommandMeta(tableMeta);
            }

            public SqlCommand BuildById<T>(object id)
            {
                if (id is T)
                {
                    return Build(id);
                }

                var cmdMeta = (SqlCommandMeta) _commands[typeof(T)];

                var paramMeta = cmdMeta.ParameterMeta.First();

                return new SqlCommand
                {
                    CommandText = cmdMeta.CommandText,
                    Parameters = new Dictionary<string, SqlParameter>
                    {
                        {
                            paramMeta.Key,
                            new SqlParameter
                            {
                                Value = id,
                                Name = paramMeta.Key,
                                ColumnMeta = paramMeta.Value
                            }
                        }
                    }
                };
            }
        }
    }
}
