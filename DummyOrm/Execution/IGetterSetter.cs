namespace DummyOrm.Execution
{
    public interface IGetterSetter
    {
        object Get(object obj);
        void Set(object obj, object value);
    }
}