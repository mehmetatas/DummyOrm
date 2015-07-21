using System;

namespace DummyOrm.ConsoleApp.Entities
{
    public class User
    {
        public virtual long Id { get; set; }
        public virtual string Fullname { get; set; }
        public virtual string Email { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual DateTime? JoinDate { get; set; }
        public virtual UserType Type { get; set; }
        public virtual UserStatus Status { get; set; }
    }
}