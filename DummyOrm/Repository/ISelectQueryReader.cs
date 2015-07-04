using System.Data;
using DummyOrm.QueryBuilders.Select;

namespace DummyOrm.Repository
{
    public interface ISelectQueryReader
    {
        IDataReader ExecuteReader(SelectQuery query);
    }
}