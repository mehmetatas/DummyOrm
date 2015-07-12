namespace DummyOrm2.Orm.Dynamix
{
    public interface IGetterSetter
    {
        object Get(object obj);
        void Set(object obj, object value);
    }
}