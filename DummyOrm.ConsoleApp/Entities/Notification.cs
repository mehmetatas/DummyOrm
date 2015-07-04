using System;

namespace DummyOrm.ConsoleApp.Entities
{
    public class Notification
    {
        public virtual long Id { get; set; }
        public virtual long ToUserId { get; set; }
        public virtual long FromUserId { get; set; }
        public virtual long PostId { get; set; }
        public virtual DateTime ActionDate { get; set; }
        public virtual string Message { get; set; }
        public virtual NotificationType Type { get; set; }
        public virtual NotificationStatus Status { get; set; }
    }
}