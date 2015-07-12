using System;

namespace DummyOrm2.Entities
{
    public class Like
    {
        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
        public virtual DateTime LikedDate { get; set; }
    }
}
