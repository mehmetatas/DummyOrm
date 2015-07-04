using System;

namespace DummyOrm.ConsoleApp.Entities
{
    public class Like
    {
        public virtual long PostId { get; set; }
        public virtual long UserId { get; set; }
        public virtual DateTime LikedDate { get; set; }
    }
}
