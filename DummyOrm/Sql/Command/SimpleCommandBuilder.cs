using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DummyOrm.Meta;

namespace DummyOrm.Sql.Command
{
    abstract class SimpleCommandBuilder : ISimpleCommandBuilder
    {
        public static ISimpleCommandBuilder Insert { get; private set; }
        public static ISimpleCommandBuilder Update { get; private set; }
        public static ISimpleCommandBuilder Delete { get; private set; }
        public static IReadCommandBuilder Select { get; private set; }

        public static void Init(IDbMeta meta)
        {
            Insert = new InsertCommandBuilder(meta);
            Update = new UpdateCommandBuilder(meta);
            Delete = new DeleteCommandBuilder(meta);
            Select = new SelectCommandBuilder(meta);
        }

        public static void RegisterAll(TableMeta tableMeta)
        {
            Insert.Register(tableMeta);
            Update.Register(tableMeta);
            Delete.Register(tableMeta);
            Select.Register(tableMeta);
        }

        private readonly Hashtable _commands = new Hashtable();

        protected SimpleCommandBuilder(IDbMeta dbMeta)
        {
            DbMeta = dbMeta;
        }

        protected IDbMeta DbMeta { get; private set; }

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
        public InsertCommandBuilder(IDbMeta dbMeta) : base(dbMeta)
        {
        }

        protected override CommandMeta CreateCommandMeta(TableMeta tableMeta)
        {
            return DbMeta.DbProvider.CreateCommandMetaBuilder().BuildInsertCommandMeta(tableMeta);
        }
    }

    class UpdateCommandBuilder : SimpleCommandBuilder
    {
        public UpdateCommandBuilder(IDbMeta dbMeta) : base(dbMeta)
        {
        }

        protected override CommandMeta CreateCommandMeta(TableMeta tableMeta)
        {
            return DbMeta.DbProvider.CreateCommandMetaBuilder().BuildUpdateCommandMeta(tableMeta);
        }
    }

    class DeleteCommandBuilder : SimpleCommandBuilder
    {
        public DeleteCommandBuilder(IDbMeta dbMeta) : base(dbMeta)
        {
        }

        protected override CommandMeta CreateCommandMeta(TableMeta tableMeta)
        {
            return DbMeta.DbProvider.CreateCommandMetaBuilder().BuildDeleteCommandMeta(tableMeta);
        }
    }

    class SelectCommandBuilder : SimpleCommandBuilder, IReadCommandBuilder
    {
        public SelectCommandBuilder(IDbMeta dbMeta) : base(dbMeta)
        {
        }

        protected override CommandMeta CreateCommandMeta(TableMeta tableMeta)
        {
            return DbMeta.DbProvider.CreateCommandMetaBuilder().BuildGetByIdCommandMeta(tableMeta);
        }

        public Command BuildById<T>(object id)
        {
            if (id is T)
            {
                return Build(id);
            }

            var cmdMeta = GetCommandMeta(typeof(T));

            var paramMeta = cmdMeta.ParameterMeta.First();

            var parameters = new Dictionary<string, CommandParameter>
            {
                {
                    paramMeta.Key,
                    new CommandParameter
                    {
                        Value = id,
                        Name = paramMeta.Key,
                        ParameterMeta = paramMeta.Value.ParameterMeta
                    }
                }
            };

            return Command.TextCommand(cmdMeta.CommandText, parameters);
        }
    }
}
