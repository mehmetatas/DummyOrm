using System.Collections.Generic;
using System.Data;
using System.Text;
using DummyOrm.Meta;

namespace DummyOrm.Sql
{
    public class CommandBuilder
    {
        private readonly StringBuilder _cmd;
        private readonly IDictionary<string, CommandParameter> _params;

        public CommandBuilder()
        {
            _cmd = new StringBuilder();
            _params = new Dictionary<string, CommandParameter>();
        }

        public CommandBuilder Append(string sql)
        {
            _cmd.Append(sql);
            return this;
        }

        public CommandBuilder AppendFormat(string format, params object[] args)
        {
            _cmd.AppendFormat(format, args);
            return this;
        }

        public CommandBuilder AddParameter(string name, object value, ParameterMeta meta = null)
        {
            if (meta == null)
            {
                meta = new ParameterMeta
                {
                    DbType = value == null ? DbType.Object : value.GetType().GetDbType()
                };
            }

            _params.Add(name, new CommandParameter
            {
                Name = name,
                Value = value,
                ParameterMeta = meta
            });

            return this;
        }

        public Command Build()
        {
            return new Command
            {
                CommandText = _cmd.ToString(),
                Parameters = _params
            };
        }
    }
}
