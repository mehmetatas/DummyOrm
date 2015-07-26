using DummyOrm.Db.Impl;
using DummyOrm.Meta;
using DummyOrm.Meta.Builders;
using DummyOrm.Providers;

namespace DummyOrm.Db
{
    public static class Db
    {
        private static IDbProvider _defaultProvider;

        public static IDbMetaBuilder Setup(IDbProvider provider)
        {
            if (_defaultProvider == null)
            {
                _defaultProvider = provider;
            }
            return new DbMetaBuilder(provider);
        }

        public static IDb Create(IDbProvider provider = null)
        {
            provider = provider ?? _defaultProvider;
            DbMeta.Push(provider.Meta);
            return new DbImpl(provider);
        }
    }
}
