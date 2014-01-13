using System;

namespace NewsFeed.Models
{
    public class Request
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SenderId { get; set; }
        public virtual User Sender { get; set; }
        public int RecipientId { get; set; }
        public virtual User Recipient { get; set; }
    }
}
