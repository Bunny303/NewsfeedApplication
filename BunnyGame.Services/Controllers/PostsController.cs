using BunnyGame.Services.Models;
using BunnyGame.Services.Persisters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BunnyGame.Services.Controllers
{
    public class PostsController : BaseApiController
    {
        [HttpPost]
        [ActionName("addPost")]
        public HttpResponseMessage AddPost(string title, string content, string sessionKey)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                UserDataPersister.CreatePost(title, content, sessionKey);
            });
            return responseMsg;
        }

        [HttpGet]
        [ActionName("getPosts")]
        public IEnumerable<PostModel> GetPosts(string sessionKey)
        {
            var result = UserDataPersister.GetAllPosts(sessionKey);
            
            return result;
        }
    }
}
