using NewsFeed.Server.Models;
using NewsFeed.Server.Persisters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NewsFeed.Server.Controllers
{
    public class UsersController : BaseApiController
    {
        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage RegisterUser(UserRegisterModel user)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                string avatar = user.Avatar;
                UserDataPersister.CreateUser(user.Username, user.AuthCode, user.Avatar);
                var sessionKey = UserDataPersister.LoginUser(user.Username, user.AuthCode, out avatar);
                return new UserLoggedModel()
                {
                    Username = user.Username,
                    SessionKey = sessionKey,
                    Avatar = user.Avatar
                };
            });

            return responseMsg;
        }
        
        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage LoginUser(UserLoginModel user)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                string avatar = string.Empty;
                var sessionKey = UserDataPersister.LoginUser(user.Username, user.AuthCode, out avatar);
                return new UserLoggedModel()
                {
                    Username = user.Username,
                    SessionKey = sessionKey,
                    Avatar = avatar
                };
            });

            return responseMsg;
        }
        
        [HttpGet]
        [ActionName("getUser")]
        public IEnumerable<UserSearchModel> GetUser(string username)
        {
            var result = UserDataPersister.GetUser(username);

            return result;
        }

        [HttpGet]
        [ActionName("getUserBySessionKey")]
        public UserLoggedModel GetUserBySessionKey(string sessionKey)
        {
            var result = UserDataPersister.GetUserBySessionKey(sessionKey);

            return result;
        }

        [HttpGet]
        [ActionName("getUserFriendsBySessionKey")]
        public IEnumerable<UserFriendModel> GetUserFriendsBySessionKey(string sessionKey)
        {
            var result = UserDataPersister.GetUserFriendsBySessionKey(sessionKey);

            return result;
        }

        [HttpGet]
        [ActionName("getUserRequestsBySessionKey")]
        public IEnumerable<ReceiveRequestModel> GetUserRequestsBySessionKey(string sessionKey)
        {
            var result = UserDataPersister.GetUserRequestsBySessionKey(sessionKey);

            return result;
        }

        [HttpGet]
        [ActionName("getUserWallBySessionKey")]
        public IEnumerable<PostModel> GetUserWallBySessionKey(string sessionKey)
        {
            var result = UserDataPersister.GetUserWallBySessionKey(sessionKey);

            return result;
        }

        [HttpPost]
        [ActionName("sendRequest")]
        public HttpResponseMessage SendRequest(UserSearchModel user, string sessionKey)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                RequestDataPersister.SendRequest(user.Username, sessionKey);
            });

            return responseMsg;
        }

        [HttpDelete]
        [ActionName("deleteRequest")]
        public HttpResponseMessage DeleteRequest(int id)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                RequestDataPersister.DeleteRequest(id);
            });

            return responseMsg;
        }

        [HttpPost]
        [ActionName("addFriend")]
        public HttpResponseMessage AddFriend(int senderId, string sessionKey)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                RequestDataPersister.AddFriend(senderId, sessionKey);
            });

            return responseMsg;
        }
    }
}
