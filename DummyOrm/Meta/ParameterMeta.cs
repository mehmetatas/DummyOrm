using System.Data;

namespace DummyOrm.Meta
{
    public class ParameterMeta
    {
        public DbType DbType { get; set; }
        public byte DecimalPrecision { get; set; }
        public int StringLength { get; set; }
    }
}