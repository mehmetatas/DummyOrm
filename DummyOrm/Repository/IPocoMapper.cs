using System.Data;

namespace DummyOrm.Repository
{
    public interface IPocoMapper
    {
        object Map(IDataReader reader);
    }
}