using DummyOrm.Db;
using DummyOrm.Meta;

namespace DummyOrm.Providers.SqlServer2012
{
    class Sql2012SchemaBuilder : ISchemaBuilder
    {
        private readonly IDbMeta _meta;

        public Sql2012SchemaBuilder(IDbMeta meta)
        {
            _meta = meta;
        }

        public void Build(SchemaBuildOptions options)
        {
            var tables = _meta.GetTables();

            foreach (var VARIABLE in ToString())
            {
                
            }
        }
    }
}
