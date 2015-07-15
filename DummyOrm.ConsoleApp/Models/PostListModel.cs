namespace DummyOrm.ConsoleApp.Models
{
    public class PostListModel
    {
        public long PostId { get; set; }
        public long UserId { get; set; }
        public string Username { get; set; }
        public string PostTitle { get; set; }
        public int TagCount { get; set; }
        public int LikeCount { get; set; }
    }
}
