
namespace DummyOrm.ConsoleApp.Entities
{
    public class FollowUser
    {
        public virtual long FollowerUserId { get; set; }
        public virtual long FollowedUserId { get; set; }
    }
}
