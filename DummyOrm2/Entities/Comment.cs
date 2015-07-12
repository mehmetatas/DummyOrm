using System;

namespace DummyOrm2.Entities
{
    public class Comment
    {
        public virtual long Id { get; set; }
        public virtual long PostId { get; set; }
        public virtual long UserId { get; set; }
        public virtual DateTime PublishDate { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
        public virtual string Content { get; set; }
    }
}