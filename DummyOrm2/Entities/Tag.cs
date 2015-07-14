
namespace DummyOrm2.Entities
{
    public class Tag
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual long Count { get; set; }
        public virtual string Hint { get; set; }
        public virtual string Description { get; set; }
    }
}