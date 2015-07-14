using System.Collections.Generic;
using DummyOrm2.Orm.Meta;

namespace DummyOrm2.Orm.Sql.Select
{
    public interface IWhereExpressionListener
    {
        Column RegisterColumn(IList<ColumnMeta> propChain);
    }
}