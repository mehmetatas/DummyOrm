using System.Collections.Generic;

namespace DummyOrm.Repository
{
    public interface IPocoReader<out T> : IEnumerable<T>
    {
        
    }
}