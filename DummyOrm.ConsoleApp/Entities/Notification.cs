using System;

namespace DummyOrm.ConsoleApp.Entities
{
    public class Notification
    {
        public virtual long Id { get; set; }
        public virtual User ToUser { get; set; }
        public virtual User FromUser { get; set; }
        public virtual Post Post { get; set; }
        public virtual DateTime ActionDate { get; set; }
        public virtual string Message { get; set; }
        public virtual NotificationType Type { get; set; }
        public virtual NotificationStatus Status { get; set; }
    }
}