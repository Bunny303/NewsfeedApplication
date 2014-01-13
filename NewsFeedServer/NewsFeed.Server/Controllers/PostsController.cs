using NewsFeed.Server.Models;
using NewsFeed.Server.Persisters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NewsFeed.Server.Controllers
{
    public class PostsController : BaseApiController
    {
        [HttpPost]
        [ActionName("addPost")]
        public HttpResponseMessage AddPost(string title, string content, string sessionKey)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                PostDataPersister.CreatePost(title, content, sessionKey);
            });
            return responseMsg;
        }

        [HttpGet]
        [ActionName("getPosts")]
        public IEnumerable<PostModel> GetPosts(string sessionKey)
        {
            var result = PostDataPersister.GetAllPosts(sessionKey);
            return result;
        }
    }
}
