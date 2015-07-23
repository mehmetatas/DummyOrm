using DummyOrm.Meta;

namespace DummyOrm.Sql
{
    public class CommandParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ParameterMeta ParameterMeta { get; set; }
    }
}