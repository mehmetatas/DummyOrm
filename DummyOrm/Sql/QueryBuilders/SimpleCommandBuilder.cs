using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DummyOrm.Sql.QueryBuilders
{
    public interface ISimpleCommandBuilder
    {
        void Register(Type type);
        SqlCommand Build(object entity);
    }
    public interface IReadCommandBuilder : ISimpleCommandBuilder
    {
        SqlCommand BuildById<T>(object id);
    }

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

        public static void RegisterAll(Type type)
        {
            Insert.Register(type);
            Update.Register(type);
            Delete.Register(type);
            Select.Register(type);
        }

        public void Register(Type type)
        {
            var cmdMeta = CreateCommandMeta(type);
            _commands.Add(type, cmdMeta);
        }

        public SqlCommand Build(object entity)
        {
            var type = entity.GetType();

            var cmdMeta = (SqlCommandMeta)_commands[type];

            return cmdMeta.CreateCommand(entity);
        }

        protected abstract SqlCommandMeta CreateCommandMeta(Type type);

        private class InsertCommandBuilder : SimpleCommandBuilder
        {
            protected override SqlCommandMeta CreateCommandMeta(Type type)
            {
                return SqlCommandMeta.CreateInsertCommandMeta(type);
            }
        }

        private class UpdateCommandBuilder : SimpleCommandBuilder
        {
            protected override SqlCommandMeta CreateCommandMeta(Type type)
            {
                return SqlCommandMeta.CreateUpdateCommandMeta(type);
            }
        }

        private class DeleteCommandBuilder : SimpleCommandBuilder
        {
            protected override SqlCommandMeta CreateCommandMeta(Type type)
            {
                return SqlCommandMeta.CreateDeleteCommandMeta(type);
            }
        }

        public class SelectCommandBuilder : SimpleCommandBuilder, IReadCommandBuilder
        {
            protected override SqlCommandMeta CreateCommandMeta(Type type)
            {
                return SqlCommandMeta.CreateSelectCommandMeta(type);
            }

            public SqlCommand BuildById<T>(object id)
            {
                var cmdMeta = (SqlCommandMeta)_commands[typeof(T)];

                return new SqlCommand(cmdMeta.CommandText, new Dictionary<string, object>
                {
                    { cmdMeta.ParameterMeta.First().Key, id }
                });
            }
        }
    }
}
