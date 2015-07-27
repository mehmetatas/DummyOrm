namespace DummyOrm.Db
{
    public interface IDbFactory
    {
        IDb Create();
    }
}
