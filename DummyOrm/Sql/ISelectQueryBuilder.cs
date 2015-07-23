namespace DummyOrm.Sql
{
    public interface ISelectQueryBuilder<T> where T : class, new()
    {
        ISelectQuery<T> Build();
    }
}