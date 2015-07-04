using DummyOrm.Sql;

namespace DummyOrm.QueryBuilders.Where
{
    public interface IWhereSqlCommandBuilder
    {
        SqlCommand Build();
    }
}