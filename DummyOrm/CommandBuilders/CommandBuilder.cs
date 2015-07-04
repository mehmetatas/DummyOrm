using System;
using DummyOrm.Sql;
using System.Collections;

namespace DummyOrm.CommandBuilders
{
    public abstract class CommandBuilder
    {
        public static readonly CommandBuilder Insert = new InsertCommandBuilder();
        public static readonly CommandBuilder Update = new UpdateCommandBuilder();
        public static readonly CommandBuilder Delete = new DeleteCommandBuilder();

        private readonly Hashtable _commands = new Hashtable();

        private CommandBuilder()
        {

        }

        public static void RegisterAll(Type type)
        {
            Insert.Register(type);
            Update.Register(type);
            Delete.Register(type);
        }

        private void Register(Type type)
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

        private class InsertCommandBuilder : CommandBuilder
        {
            protected override SqlCommandMeta CreateCommandMeta(Type type)
            {
                return SqlCommandMeta.CreateInsertCommandMeta(type);
            }
        }

        private class UpdateCommandBuilder : CommandBuilder
        {
            protected override SqlCommandMeta CreateCommandMeta(Type type)
            {
                return SqlCommandMeta.CreateUpdateCommandMeta(type);
            }
        }

        private class DeleteCommandBuilder : CommandBuilder
        {
            protected override SqlCommandMeta CreateCommandMeta(Type type)
            {
                return SqlCommandMeta.CreateDeleteCommandMeta(type);
            }
        }
    }
}
