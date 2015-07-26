﻿using System.Data;
using DummyOrm.Meta;
using DummyOrm.Sql.Command;
using DummyOrm.Sql.Delete;
using DummyOrm.Sql.Select;
using DummyOrm.Sql.Where;

namespace DummyOrm.Providers
{
    public interface IDbProvider
    {
        char QuoteOpen { get; }

        char QuoteClose { get; }

        char ParameterPrefix { get; }

        IDbMeta Meta { get; }

        ISelectCommandBuilder CreateSelectCommandBuilder();

        IWhereCommandBuilder CreateWhereCommandBuilder();

        ICommandMetaBuilder CreateCommandMetaBuilder();

        IDeleteManyCommandBuilder CreateDeleteManyCommandBuilder();

        IDbConnection CreateConnection();
    }
}
