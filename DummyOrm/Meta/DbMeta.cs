using System.Collections.Generic;
using DummyOrm.Execution;
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
                TableName = type.Name,
                Factory = PocoFactory.CreateFactory(type)
            };

            var props =
                type.GetProperties()
                    .Where(p => !typeof(IEnumerable).IsAssignableFrom(p.PropertyType) ||
                                p.PropertyType == typeof(string) ||
                                p.PropertyType == typeof(byte[]))
                    .ToList();

            var idProp = props.FirstOrDefault(p => p.Name == "Id");

            tableMeta.AssociationTable = idProp == null;

            var columns = new List<ColumnMeta>();

            foreach (var prop in props)
            {
                var isReference = prop.PropertyType.IsClass &&
                                  prop.PropertyType != typeof(string) &&
                                  prop.PropertyType != typeof(byte[]);

                var columnMeta = new ColumnMeta
                {
                    Table = tableMeta,
                    Property = prop,
                    ColumnName = prop.Name,
                    IsRefrence = isReference,
                    GetterSetter = GetterSetter.Create(prop)
                };

                if (isReference)
                {
                    columnMeta.ColumnName += "Id";
                }

                //columnMeta.Column = new Column
                //{
                //    Table = tableMeta.TableName,
                //    ColumnName = columnMeta.ColumnName
                //};

                if (tableMeta.AssociationTable)
                {
                    columnMeta.Identity = isReference;
                    columnMeta.AutoIncrement = false;
                }
                else
                {
                    columnMeta.Identity = columnMeta.ColumnName == "Id";
                    columnMeta.AutoIncrement = columnMeta.ColumnName == "Id";
                }

                columns.Add(columnMeta);
            }

            tableMeta.Columns = columns.ToArray();

            if (!tableMeta.AssociationTable)
            {
                tableMeta.IdColumn = columns.First(c => c.ColumnName == "Id");
            }

            foreach (var column in columns)
            {
                _columns.Add(column.Property, column);
            }

            _tables.Add(type, tableMeta);

            //SimpleCommandBuilder.RegisterAll(type);

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