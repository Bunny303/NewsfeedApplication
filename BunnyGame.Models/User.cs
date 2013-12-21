using System;
using System.Collections.Generic;

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

        public User()
        {
            this.Wall = new HashSet<Post>();
        }
    }
}
