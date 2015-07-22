using System;
using System.Collections;
using DummyOrm.Dynamix;

namespace DummyOrm.Meta
{
    public class OneToManyMeta : IAssociationMeta
    {
        public Func<IList> ListFactory { get; set; }
        public IGetterSetter ListGetterSetter { get; set; }
        public ColumnMeta ForeignKey { get; set; }
        public ColumnMeta PrimaryKey { get; set; }
        public IAssociationLoader Loader { get; set; }
    }
}