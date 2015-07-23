using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DummyOrm.Meta;

namespace DummyOrm.Sql.SimpleCommands
{
    abstract class SimpleCommandBuilder : ISimpleCommandBuilder
    {
        public static readonly ISimpleCommandBuilder Insert = new InsertCommandBuilder();
        public static readonly ISimpleCommandBuilder Update = new UpdateCommandBuilder();
        public static readonly ISimpleCommandBuilder Delete = new DeleteCommandBuilder();
        public static readonly IReadCommandBuilder Select = new SelectCommandBuilder();

        public static void RegisterAll(TableMeta tableMeta)
        {
            Insert.Register(tableMeta);
            Update.Register(tableMeta);
            Delete.Register(tableMeta);
            Select.Register(tableMeta);
        }

        private readonly Hashtable _commands = new Hashtable();

        protected CommandMeta GetCommandMeta(Type type)
        {
            return (CommandMeta)_commands[type];
        }

        public void Register(TableMeta tableMeta)
        {
            var cmdMeta = CreateCommandMeta(tableMeta);
            _commands.Add(tableMeta.Type, cmdMeta);
        }

        public Command Build(object entity)
        {
            var type = entity.GetType();

            var cmdMeta = GetCommandMeta(type);

            return cmdMeta.CreateCommand(entity);
        }

        protected abstract CommandMeta CreateCommandMeta(TableMeta tableMeta);
    }

    class InsertCommandBuilder : SimpleCommandBuilder
    {
        protected override CommandMeta CreateCommandMeta(TableMeta tableMeta)
        {
            return DbMeta.Instance.DbProvider.CreateCommandMetaBuilder().BuildInsertCommandMeta(tableMeta);
        }
    }

    class UpdateCommandBuilder : SimpleCommandBuilder
    {
        protected override CommandMeta CreateCommandMeta(TableMeta tableMeta)
        {
            return DbMeta.Instance.DbProvider.CreateCommandMetaBuilder().BuildUpdateCommandMeta(tableMeta);
        }
    }

    class DeleteCommandBuilder : SimpleCommandBuilder
    {
        protected override CommandMeta CreateCommandMeta(TableMeta tableMeta)
        {
            return DbMeta.Instance.DbProvider.CreateCommandMetaBuilder().BuildDeleteCommandMeta(tableMeta);
        }
    }

    class SelectCommandBuilder : SimpleCommandBuilder, IReadCommandBuilder
    {
        protected override CommandMeta CreateCommandMeta(TableMeta tableMeta)
        {
            return DbMeta.Instance.DbProvider.CreateCommandMetaBuilder().BuildGetByIdCommandMeta(tableMeta);
        }

        public Command BuildById<T>(object id)
        {
            if (id is T)
            {
                return Build(id);
            }

            var cmdMeta = GetCommandMeta(typeof(T));

            var paramMeta = cmdMeta.ParameterMeta.First();

            return new Command
            {
                CommandText = cmdMeta.CommandText,
                Parameters = new Dictionary<string, CommandParameter>
                    {
                        {
                            paramMeta.Key,
                            new CommandParameter
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
