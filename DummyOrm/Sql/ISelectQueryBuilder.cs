using DummyOrm.Sql.Select;

namespace DummyOrm.Sql
{
    public interface ISelectQueryBuilder<T> where T : class, new()
    {
        SelectQuery<T> Build();
    }
}