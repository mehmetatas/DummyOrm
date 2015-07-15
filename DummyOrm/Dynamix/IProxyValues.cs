using System.Collections.Generic;

namespace DummyOrm.Dynamix
{
    public interface IProxyValues
    {
        IDictionary<string, object> Values { get; }
    }
}