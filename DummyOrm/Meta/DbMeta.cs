using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DummyOrm.Dynamix.Impl;
using DummyOrm.Providers;
using DummyOrm.Sql.Command;

namespace DummyOrm.Meta
{
    public interface IDbMeta
    {
        IDbProvider DbProvider { get; }

        TableMeta RegisterEntity(Type type);

        TableMeta GetTable(Type type);

        ColumnMeta GetColumn(PropertyInfo prop);

        IAssociationMeta GetAssociation(PropertyInfo prop);

        ManyToManyMeta ManyToMany<TParent, TAssoc>(Expression<Func<TParent, IList>> listPropExp)
            where TParent : class, new()
            where TAssoc : class, new();

        OneToManyMeta OneToMany<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> listPropExp,
            Expression<Func<TMany, TOne>> foreignPropExp)
            where TOne : class, new()
            where TMany : class, new();
    }

    class DbMeta : IDbMeta
    {
        private readonly Hashtable _tables = new Hashtable();
        private readonly Hashtable _columns = new Hashtable();
        private readonly Hashtable _associations = new Hashtable();

        public static IDbMeta Current
        {
            get { return Stack.Peek(); }
        }

        public IDbProvider DbProvider { get; private set; }

        public DbMeta (IDbProvider provider)
        {
            DbProvider = provider;
        }

        public OneToManyMeta OneToMany<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> listPropExp, Expression<Func<TMany, TOne>> foreignPropExp)
            where TOne : class, new()
            where TMany : class, new()
        {
            EnsureReferences();

            var parentTable = this.GetTable<TOne>();
            var primaryKey = parentTable.IdColumn;

            var listProp = listPropExp.GetPropertyInfo();

            var childCol = GetColumn(foreignPropExp.GetPropertyInfo());

            var assoc = new OneToManyMeta
            {
                ListFactory = PocoFactory.CreateListFactory(listProp.PropertyType),
                ListGetterSetter = GetterSetter.Create(listProp),
                PrimaryKey = primaryKey,
                ForeignKey = childCol
            };

            assoc.Loader = new OneToManyLoader(assoc);

            _associations.Add(listProp, assoc);

            return assoc;
        }

        public ManyToManyMeta ManyToMany<TParent, TAssoc>(Expression<Func<TParent, IList>> listPropExp)
            where TParent : class, new()
            where TAssoc : class, new()
        {
            EnsureReferences();

            var listProp = listPropExp.GetPropertyInfo();

            var parentType = listProp.DeclaringType;
            var childType = listProp.PropertyType.GetGenericArguments()[0];

            var parentTable = GetTable(parentType);
            var childTable = GetTable(childType);
            var assocTable = this.GetTable<TAssoc>();

            var parentColumn = assocTable.Columns.First(c => c.ReferencedTable == parentTable);
            var childColumn = assocTable.Columns.First(c => c.ReferencedTable == childTable);

            var assoc = new ManyToManyMeta
            {
                ListFactory = PocoFactory.CreateListFactory(listProp.PropertyType),
                ListGetterSetter = GetterSetter.Create(listProp),
                ChildColumn = childColumn,
                ParentColumn = parentColumn
            };

            assoc.Loader = new ManyToManyLoader<TAssoc>(assoc);

            _associations.Add(listProp, assoc);

            return assoc;
        }

        public DbMeta RegisterModel(Type type)
        {
            PocoDeserializer.RegisterModel(type);
            return this;
        }

        public TableMeta RegisterEntity(Type type)
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
                    GetterSetter = GetterSetter.Create(prop),
                    ParameterMeta = new ParameterMeta
                    {
                        DbType = isReference ? DbType.Int64 : prop.PropertyType.GetDbType()
                    }
                };

                if (isReference)
                {
                    columnMeta.ColumnName += "Id";
                    columnMeta.Loader = new OneToOneLoader(columnMeta);
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

            EnsureReferences();

            return tableMeta;
        }

        public TableMeta GetTable(Type type)
        {
            return (TableMeta)_tables[type];
        }

        public ColumnMeta GetColumn(PropertyInfo prop)
        {
            return (ColumnMeta)_columns[prop];
        }

        public IAssociationMeta GetAssociation(PropertyInfo prop)
        {
            var assoc = _associations[prop] as IAssociationMeta;

            if (assoc == null)
            {
                throw new InvalidOperationException("Association meta not found for " + prop);
            }

            return assoc;
        }

        private void EnsureReferences()
        {
            foreach (var table in _tables.Values.OfType<TableMeta>())
            {
                foreach (var column in table.Columns.Where(c => c.IsRefrence))
                {
                    column.ReferencedTable = GetTable(column.Property.PropertyType);
                }
            }
        }

        // TODO: Make it web thread safe
        [ThreadStatic]
        private static Stack<IDbMeta> _stack;
        private static Stack<IDbMeta> Stack
        {
            get
            {
                if (_stack == null)
                {
                    _stack = new Stack<IDbMeta>();
                }
                return _stack;
            }
        }

        public static void Push(IDbMeta meta)
        {
            Stack.Push(meta);
        }

        public static void Pop()
        {
            Stack.Pop();
        }
    }
}