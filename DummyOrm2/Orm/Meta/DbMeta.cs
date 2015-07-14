using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DummyOrm2.Orm.Dynamix;

namespace DummyOrm2.Orm.Meta
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

            var props = type.GetProperties()
                .Where(p => p.IsColumnProperty())
                .ToList();

            var idProp = props.FirstOrDefault(p => p.Name == "Id");

            tableMeta.AssociationTable = idProp == null;

            var columns = new List<ColumnMeta>();

            foreach (var prop in props)
            {
                var isReference = prop.IsReferenceProperty();

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
}