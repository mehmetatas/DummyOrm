﻿using System.Collections.Generic;
using DummyOrm2.Orm.Meta;
using DummyOrm2.Orm.Sql.Select;

namespace DummyOrm2.Orm.Sql
{
    public class SqlParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ColumnMeta ColumnMeta { get; set; }
    }

    public class SqlCommand
    {
        public string CommandText { get; set; }
        public IDictionary<string, SqlParameter> Parameters { get; set; }
    }

    public interface ISelectQueryBuilder<T> where T : class, new()
    {
        SelectQuery<T> Build();
    }

    public interface ISelectSqlCommandBuilder
    {
        SqlCommand Build<T>(SelectQuery<T> query) where T : class, new();
    }
}