using System.Collections.Generic;

namespace DummyOrm2.Orm.Dynamix
{
    public interface IFilterValues
    {
        IDictionary<string, object> Values { get; }
    }
}