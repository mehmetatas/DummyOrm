using System;
using System.Collections;
using DummyOrm.Dynamix;

namespace DummyOrm.Meta
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
}