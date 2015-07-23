namespace DummyOrm.Db.Impl
{
    public static class DbFactory
    {
        public static IDb Create()
        {
            return new DbImpl();
        }
    }
}
