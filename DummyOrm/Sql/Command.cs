using System.Collections.Generic;

namespace DummyOrm.Sql
{
    public class Command
    {
        public string CommandText { get; set; }
        public IDictionary<string, CommandParameter> Parameters { get; set; }
    }
}