using System;
using System.Collections.Generic;

namespace DummyOrm.ConsoleApp.Entities
{
    public class Post
    {
        public virtual long Id { get; set; }
        public virtual long UserId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime? PublishDate { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
        public virtual string Title { get; set; }
        public virtual string HtmlContent { get; set; }
        public virtual AccessLevel AccessLevel { get; set; }
        public virtual byte[] Data { get; set; }

        public virtual List<Tag> Tags { get; set; }
        public virtual List<Like> Likes { get; set; }
    }
}