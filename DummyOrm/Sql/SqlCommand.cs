using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DummyOrm.Sql
{
    public class SqlCommand
    {
        public SqlCommand(string commandText, IDictionary<string, object> parameters)
        {
            CommandText = commandText;
            Parameters = new ReadOnlyDictionary<string, object>(parameters);
        }

        public string CommandText { get; private set; }
        public IDictionary<string, object> Parameters { get; private set; }
    }
}
