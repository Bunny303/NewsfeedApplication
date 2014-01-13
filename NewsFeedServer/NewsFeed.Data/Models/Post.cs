using System;

namespace NewsFeed.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? PublicDate { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
