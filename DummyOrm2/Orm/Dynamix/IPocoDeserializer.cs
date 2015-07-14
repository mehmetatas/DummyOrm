using System.Data;

namespace DummyOrm2.Orm.Dynamix
{
    public interface IPocoDeserializer
    {
        object Deserialize(IDataReader reader);
    }
}