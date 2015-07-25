using System.Collections.Generic;
using DummyOrm.Meta;

namespace DummyOrm.Sql.Where
{
    public interface IWhereExpressionListener
    {
        Column RegisterColumn(IList<ColumnMeta> propChain);
    }
}