using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DummyOrm.Dynamix.Impl;
using DummyOrm.Sql.SimpleCommands;

namespace DummyOrm.Meta
{
    public class DbMeta
    {
        private readonly Hashtable _tables = new Hashtable();
        private readonly Hashtable _columns = new Hashtable();
        private readonly Hashtable _associations = new Hashtable();

        public static readonly DbMeta Instance = new DbMeta();

        private DbMeta()
        {

        }

        public TableMeta GetTable(Type type)
        {
            return (TableMeta)_tables[type];
        }

        public ColumnMeta GetColumn(PropertyInfo prop)
        {
            return (ColumnMeta)_columns[prop];
        }

        public AssociationMeta GetAssociation(PropertyInfo prop)
        {
            return (AssociationMeta)_associations[prop];
        }

        public DbMeta RegisterModel(Type type)
        {
            PocoDeserializer.RegisterModel(type);
            return this;
        }

        public DbMeta RegisterEntity(Type type)
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
                    DbType = isReference ? DbType.Int64 : prop.PropertyType.GetDbType(),
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
            
            SimpleCommandBuilder.RegisterAll(tableMeta);
            PocoDeserializer.RegisterEntity(type);

            return this;
        }

        public DbMeta BuildRelations()
        {
            BuildReferences();
            BuildAssociations();

            return this;
        }

        private void BuildAssociations()
        {
            var allTables = _tables.Values.OfType<TableMeta>().ToArray();
            foreach (var parentTable in allTables)
            {
                var parentType = parentTable.Type;

                var props = parentType.GetProperties()
                    .Where(p => !p.IsColumnProperty() &&
                                p.PropertyType.IsGenericType &&
                                p.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    .ToList();

                var associations = new List<AssociationMeta>();

                foreach (var prop in props)
                {
                    var childType = prop.PropertyType.GetGenericArguments()[0];
                    var childTable = GetTable(childType);

                    var assocTable = allTables.FirstOrDefault(t =>
                        t.AssociationTable &&
                        t.Columns.Any(c => c.Identity && c.IsRefrence && c.ReferencedTable == parentTable) &&
                        t.Columns.Any(c => c.Identity && c.IsRefrence && c.ReferencedTable == childTable));

                    if (assocTable == null)
                    {
                        continue;
                    }

                    var parentColumn = assocTable.Columns.First(c => c.Identity && c.IsRefrence && c.ReferencedTable == parentTable);
                    var childColumn = assocTable.Columns.First(c => c.Identity && c.IsRefrence && c.ReferencedTable == childTable);

                    var assoc = new AssociationMeta
                    {
                        ListFactory = PocoFactory.CreateListFactory(prop.PropertyType),
                        ListGetterSetter = GetterSetter.Create(prop),
                        ChildColumn = childColumn,
                        ParentColumn = parentColumn
                    };

                    assoc.Loader = new AssociationLoader(assoc);

                    associations.Add(assoc);
                    _associations.Add(prop, assoc);
                }

                parentTable.Associations = associations.ToArray();
            }
        }

        private void BuildReferences()
        {
            foreach (var table in _tables.Values.OfType<TableMeta>())
            {
                foreach (var column in table.Columns.Where(c => c.IsRefrence))
                {
                    column.ReferencedTable = GetTable(column.Property.PropertyType);
                }
            }
        }
    }
}