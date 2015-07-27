using DummyOrm.Meta;

namespace DummyOrm.Db.Impl
{
    class DbFactoryImpl : IDbFactory
    {
        private readonly IDbMeta _meta;

        public DbFactoryImpl(IDbMeta meta)
        {
            _meta = meta;
        }

        public IDb Create()
        {
            return new DbImpl(_meta);
        }
    }
}
