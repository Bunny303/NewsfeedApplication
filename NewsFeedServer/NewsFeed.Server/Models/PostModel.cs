using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NewsFeed.Server.Models
{
    public class PostModel
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "date")]
        public DateTime? PublicDate { get; set; }

        [DataMember(Name = "author")]
        public string Author { get; set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }

    }
}