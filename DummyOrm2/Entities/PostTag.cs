namespace DummyOrm2.Entities
{
    public class PostTag
    {
        public virtual Post Post { get; set; }
        public virtual Tag Tag { get; set; }
    }
}