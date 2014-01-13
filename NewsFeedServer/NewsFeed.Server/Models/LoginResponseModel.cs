using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NewsFeed.Server.Models
{
    [DataContract]
    public class LoginResponseModel
    {
        [DataMember(Name = "sessionKey")]
        public string SessionKey { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }
    }
}