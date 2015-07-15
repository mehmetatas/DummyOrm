namespace DummyOrm.ConsoleApp.Entities
{
    public class PostTag
    {
        public virtual Post Post { get; set; }
        public virtual Tag Tag { get; set; }
    }
}