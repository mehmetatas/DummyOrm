using DummyOrm.Sql.QueryBuilders;
using DummyOrm.Sql.QueryBuilders.Select;
using DummyOrm.Sql;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DummyOrm.Utils;

namespace DummyOrm.Meta
{
    public class DbMeta
    {
        private readonly Hashtable _tables = new Hashtable();
        private readonly Hashtable _columns = new Hashtable();

        public static readonly DbMeta Instance = new DbMeta();

        private DbMeta()
        {

        }

        public DbMeta Register(Type type)
        {
            var tableMeta = new TableMeta
            {
                Type = type,
                TableName = type.Name
            };

            var props =
                type.GetProperties()
                    .Where(p => p.PropertyType.IsValueType ||
                                p.PropertyType == typeof(string) ||
                                p.PropertyType == typeof(byte[]))
                    .ToList();

            var idProp = props.FirstOrDefault(p => p.Name == "Id");

            tableMeta.AssociationTable = idProp == null;

            if (tableMeta.AssociationTable)
            {
                tableMeta.Columns = props.Select(prop => new ColumnMeta
                {
                    Table = tableMeta,
                    ColumnName = prop.Name,
                    Property = prop,
                    Identity = prop.Name.EndsWith("Id"),
                    AutoIncrement = false,
                    Column = new Column
                    {
                        Table = tableMeta.TableName,
                        ColumnName = prop.Name
                    }
                }).ToArray();
            }
            else
            {
                tableMeta.Columns = props.Select(prop => new ColumnMeta
                {
                    Table = tableMeta,
                    ColumnName = prop.Name,
                    Property = prop,
                    Identity = prop.Name == "Id",
                    AutoIncrement = prop.Name == "Id",
                    Column = new Column
                    {
                        Table = tableMeta.TableName,
                        ColumnName = prop.Name
                    }
                }).ToArray();

                tableMeta.IdColumn = tableMeta.Columns.First(c => c.ColumnName == "Id");
            }

            foreach (var column in tableMeta.Columns)
            {
                _columns.Add(column.Property, column);
            }

            _tables.Add(type, tableMeta);

            SimpleCommandBuilder.RegisterAll(type);

            return this;
        }

        public TableMeta GetTable(Type type)
        {
            return (TableMeta)_tables[type];
        }

        public ColumnMeta GetColumn(PropertyInfo prop)
        {
            return (ColumnMeta)_columns[prop];
        }
    }

    public static class DbMetaExtenisons
    {
        public static TableMeta GetTable<T>(this DbMeta dbMeta)
        {
            return dbMeta.GetTable(typeof(T));
        }

        public static DbMeta Register<T>(this DbMeta dbMeta)
        {
            return dbMeta.Register(typeof(T));
        }

        public static ColumnMeta GetColumn<T, TProp>(this DbMeta dbMeta, Expression<Func<T, TProp>> propExp)
        {
            return dbMeta.GetColumn(propExp.GetPropertyInfo());
        }
    }
}