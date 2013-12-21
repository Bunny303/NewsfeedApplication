using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BunnyGame.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string AuthenticationCode { get; set; }
        public string SessionKey { get; set; }
        public string Avatar { get; set; }
        

        public virtual ICollection<Post> Wall { get; set; }
        public virtual ICollection<Friend> Friends { get; set; }
        [InverseProperty("Sender")]
        public virtual ICollection<Request> SentRequests { get; set; }
        [InverseProperty("Recipient")]
        public virtual ICollection<Request> ReceiveRequests { get; set; }
        

        public User()
        {
            this.Wall = new HashSet<Post>();
            this.Friends = new HashSet<Friend>();
            this.SentRequests = new HashSet<Request>();
            this.ReceiveRequests = new HashSet<Request>();
        }

        //Create many to maney realtionship self-joined for Frieds?
    }
}
