using System;

namespace DummyOrm.ConsoleApp.Entities
{
    public class PrivateMessage
    {
        public virtual long Id { get; set; }
        public virtual User ToUser { get; set; }
        public virtual User FromUser { get; set; }
        public virtual DateTime MessageDate { get; set; }
        public virtual string Message { get; set; }
        public virtual PrivateMessageStatus Status { get; set; }
    }
}