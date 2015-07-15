using System;

namespace DummyOrm.ConsoleApp.Entities
{
    public class Comment
    {
        public virtual long Id { get; set; }
        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
        public virtual DateTime PublishDate { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
        public virtual string Content { get; set; }
    }
}