using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NewsFeed.Server.Models
{
    [DataContract]
    public class UserLoginModel
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "authCode")]
        public string AuthCode { get; set; }
    }


    [DataContract]
    public class UserRegisterModel:UserLoginModel
    {
        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }
    }

    [DataContract]
    public class UserSearchModel
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }
    }

    [DataContract]
    public class UserFriendModel: UserSearchModel
    {
        [DataMember(Name = "wall")]
        public List<PostModel> Wall { get; set; }
    }

    [DataContract]
    public class UserLoggedModel
    {
        [DataMember(Name = "sessionKey")]
        public string SessionKey { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }

    }
      
}