using System.Collections.Generic;
using DummyOrm.Meta;
using DummyOrm.Sql.Select;

namespace DummyOrm.Sql
{
    public interface IWhereExpressionListener
    {
        Column RegisterColumn(IList<ColumnMeta> propChain);
    }
}