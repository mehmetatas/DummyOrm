using System.Collections.Generic;

namespace DummyOrm.Dynamix.Impl
{
    public class ProxyValues : IProxyValues
    {
        public IDictionary<string, object> Values { get; private set; }

        public ProxyValues()
        {
            Values = new Dictionary<string, object>();
        }

        public void SetValue(string prop, object value)
        {
            if (Values.ContainsKey(prop))
            {
                Values[prop] = value;
            }
            else
            {
                Values.Add(prop, value);
            }
        }
    }
}