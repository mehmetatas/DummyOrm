using System.Collections.Generic;
using DummyOrm.Meta;

namespace DummyOrm.Sql.Select
{
    public interface IWhereExpressionListener
    {
        Column RegisterColumn(IList<ColumnMeta> propChain);
    }
}