using BunnyGame.Data;
using BunnyGame.Models;
using BunnyGame.Services.Models;
using BunnyGame.Services.Persisters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace BunnyGame.Services.Controllers
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
        //[HttpPut]
        //[ActionName("logout")]
        //public HttpResponseMessage LogoutUser(string sessionKey)
        //{
        //    var responseMsg = this.PerformOperation(() =>
        //    {
        //        UserDataPersister.LogoutUser(sessionKey);
        //    });
        //    return responseMsg;
        //}

        [HttpGet]
        [ActionName("getUser")]
        public UserSearchModel GetUser(string username)
        {
            var result = UserDataPersister.GetUser(username);

            return result;
        }
    }
}
