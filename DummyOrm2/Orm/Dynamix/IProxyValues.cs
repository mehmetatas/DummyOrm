using System.Collections.Generic;

namespace DummyOrm2.Orm.Dynamix
{
    public interface IProxyValues
    {
        IDictionary<string, object> Values { get; }
    }
}