using System.Collections.Generic;

namespace DummyOrm2.Orm.Db
{
    public interface IFillQuery<in T>
    {
        void Of(IEnumerable<T> entities);
    }
}