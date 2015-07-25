namespace DummyOrm.Sql.Select
{
    public interface ISelectQueryBuilder<T> where T : class, new()
    {
        ISelectQuery Build();
    }
}