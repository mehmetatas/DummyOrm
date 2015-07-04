using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace DummyOrm.Sql
{
    public class SqlCommand
    {
        public SqlCommand(string commandText, IDictionary<string, SqlCommandParameter> parameters)
        {
            CommandText = commandText;
            Parameters = new ReadOnlyDictionary<string, SqlCommandParameter>(parameters);
        }

        public string CommandText { get; private set; }
        public IDictionary<string, SqlCommandParameter> Parameters { get; private set; }
    }

    public class SqlCommandParameter
    {
        public static readonly IDictionary<Type, DbType> TypeMap = new Dictionary<Type, DbType>
        {
            { typeof(string), DbType.String },
            { typeof(byte[]), DbType.Binary },
            { typeof(int), DbType.Int32},
            { typeof(long), DbType.Int64},
            { typeof(bool), DbType.Boolean },
            { typeof(DateTime), DbType.DateTime },
            { typeof(int?), DbType.Int32},
            { typeof(long?), DbType.Int64},
            { typeof(bool?), DbType.Boolean },
            { typeof(DateTime?), DbType.DateTime }
        };

        public string Name { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }

        public DbType DbType
        {
            get
            {
                if (Type.IsEnum)
                {
                    return DbType.Int32;
                }
                if (TypeMap.ContainsKey(Type))
                {
                    return TypeMap[Type];
                }
                throw new NotSupportedException("Property type not supported: " + Type);
            }
        }
    }
}
