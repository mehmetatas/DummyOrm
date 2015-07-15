using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DummyOrm2.Orm.Db;
using DummyOrm2.Orm.Dynamix;
using DummyOrm2.Orm.Sql;

namespace DummyOrm2.Orm.Meta
{
    public class AssociationMeta
    {
        public Func<IList> ListFactory { get; set; }
        public IGetterSetter ListGetterSetter { get; set; }
        public ColumnMeta ParentColumn { get; set; }
        public ColumnMeta ChildColumn { get; set; }
        public IAssociationLoader Loader { get; set; }

        public override string ToString()
        {
            return ParentColumn.Table.ToString();
        }
    }

    public interface IAssociationLoader
    {
        void Load<T>(IList<T> entities, ICommandExecutor cmdExec) where T : class, new();
    }

    public class AssociationLoader : IAssociationLoader
    {
        private readonly AssociationMeta _meta;
        private readonly string _selectTemplate;
        private readonly IPocoDeserializer _childDeserializer;

        public AssociationLoader(AssociationMeta meta)
        {
            _meta = meta;
            _selectTemplate = InitCommandTemplate();
            _childDeserializer = PocoDeserializer.GetDefault(_meta.ChildColumn.ReferencedTable.Type);
        }

        private string InitCommandTemplate()
        {
            var assocTable = _meta.ChildColumn.Table;
            var childTable = _meta.ChildColumn.ReferencedTable;

            // SELECT asoc.parentId, child.* FROM child JOIN assoc ON child.Id = assoc.childId WHERE assoc.parentId IN (...)
            return new StringBuilder()
                .AppendFormat("SELECT [{0}].[{1}],", assocTable.TableName, _meta.ParentColumn.ColumnName)
                .AppendLine(String.Join(",", childTable.Columns.Select(c => String.Format("[{0}].[{1}]", c.Table.TableName, c.ColumnName))))
                .AppendFormat("FROM [{0}] JOIN [{1}] ON [{0}].[{2}] = [{1}].[{3}]", childTable.TableName, assocTable.TableName, childTable.IdColumn.ColumnName, _meta.ChildColumn.ColumnName)
                .AppendLine()
                .AppendFormat("WHERE [{0}].[{1}] IN (", assocTable.TableName, _meta.ParentColumn.ColumnName)
                .Append("{0})")
                .ToString();
        }

        public void Load<T>(IList<T> parentEntities, ICommandExecutor cmdExec) where T : class, new()
        {
            var parentId = _meta.ParentColumn.ReferencedTable.IdColumn;
            var parentIdGetter = parentId.GetterSetter;

            var inParams = new StringBuilder();
            var parameters = new Dictionary<string, SqlParameter>();

            var comma = "";
            foreach (var parentEntity in parentEntities)
            {
                var value = parentIdGetter.Get(parentEntity);
                var paramName = String.Format("p{0}", parameters.Count);
                parameters.Add(paramName, new SqlParameter
                {
                    Name = paramName,
                    Value = value,
                    ColumnMeta = parentId
                });
                inParams.Append(comma).AppendFormat("@{0}", paramName);
                comma = ",";
            }

            var cmd = new SqlCommand
            {
                CommandText = String.Format(_selectTemplate, inParams),
                Parameters = parameters
            };

            using (var reader = cmdExec.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var parentIdValue = reader[_meta.ParentColumn.ColumnName];
                    var parentEntity = parentEntities.First(pe => parentIdGetter.Get(pe).Equals(parentIdValue));

                    var list = (IList)_meta.ListGetterSetter.Get(parentEntity);
                    if (list == null)
                    {
                        list = _meta.ListFactory();
                        _meta.ListGetterSetter.Set(parentEntity, list);
                    }

                    var child = _childDeserializer.Deserialize(reader);
                    list.Add(child);
                }
            }
        }
    }
}