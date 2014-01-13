using System;

namespace NewsFeed.Models
{
    public class Friend
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
