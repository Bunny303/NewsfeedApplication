using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BunnyGame.Services.Models
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
    //[DataContract]
    //public class UserModel
    //{
    //    [DataMember(Name = "id")]
    //    public int Id { get; set; }

    //    [DataMember(Name = "username")]
    //    public string Username { get; set; }

    //    [DataMember(Name = "avatar")]
    //    public string Avatar { get; set; }

    //    [DataMember(Name = "wall")]
    //    public List<PostModel> Wall { get; set; }
    //}

    [DataContract]
    public class UserLoggedModel
    {
        [DataMember(Name = "sessionKey")]
        public string SessionKey { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }

        [DataMember(Name = "wall")]
        public List<PostModel> Wall { get; set; }

        [DataMember(Name = "friends")]
        public List<UserFriendModel> Friends { get; set; }

        [DataMember(Name = "receiveRequests")]
        public List<ReceiveRequestModel> ReceiveRequests { get; set; }
    }

    
}