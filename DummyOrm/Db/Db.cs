using DummyOrm.Db.Builders;
using DummyOrm.Db.Builders.Impl;
using DummyOrm.Providers;

namespace DummyOrm.Db
{
    public static class Db
    {
        private static IDbProvider _defaultProvider;

        public static IDbBuilder Setup(IDbProvider provider)
        {
            if (_defaultProvider == null)
            {
                _defaultProvider = provider;
            }

            return new DbBuilder(provider);
        }
    }
}
