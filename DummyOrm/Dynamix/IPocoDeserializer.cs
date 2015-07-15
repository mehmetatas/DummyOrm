using System.Data;

namespace DummyOrm.Dynamix
{
    public interface IPocoDeserializer
    {
        object Deserialize(IDataReader reader);
    }
}