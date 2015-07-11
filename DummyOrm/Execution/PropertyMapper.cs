using System;
using System.Collections.Generic;
using System.Linq;
using DummyOrm.Meta;

namespace DummyOrm.Execution
{
    public class PropertyMapper
    {
        public IEnumerable<ColumnMeta> PropertyChain { get; private set; }

        public PropertyMapper(IEnumerable<ColumnMeta> propertyChain)
        {
            PropertyChain = propertyChain;
        }

        public void Map(object entity, object value)
        {
            foreach (var colMeta in PropertyChain)
            {
                if (colMeta.IsRefrence)
                {
                    var childObj = colMeta.GetValue(entity);
                    
                    if (childObj == null)
                    {
                        var refTable = colMeta.GetReferencedTable();
                        childObj = refTable.Factory();
                        colMeta.SetValue(entity, childObj);

                        if (colMeta == PropertyChain.Last())
                        {
                            refTable.IdColumn.SetValue(childObj, value);
                        }
                    }

                    entity = childObj;
                }
                else
                {
                    colMeta.SetValue(entity, value);
                }
            }
        }

        public override string ToString()
        {
            return PropertyChain.First().Table.Type.Name + "." +
                   String.Join(".", PropertyChain.Select(c => c.Property.Name)); ;
        }
    }
}