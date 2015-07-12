using System.Collections.Generic;

namespace DummyOrm2.Orm.Dynamix
{
    public class FilterValues : IFilterValues
    {
        public IDictionary<string, object> Values { get; private set; }

        public FilterValues()
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