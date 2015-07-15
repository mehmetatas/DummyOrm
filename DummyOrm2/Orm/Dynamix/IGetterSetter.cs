namespace DummyOrm2.Orm.Dynamix
{
    public interface IGetter
    {
        object Get(object obj);
    }

    public interface ISetter
    {
        void Set(object obj, object value);
    }

    public interface IGetterSetter : IGetter, ISetter
    {
    }
}