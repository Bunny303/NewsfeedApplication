using NewsFeed.Data;
using NewsFeed.Models;
using NewsFeed.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsFeed.Server.Persisters
{
    public class PostDataPersister: BaseDataPersister
    {
        public static IEnumerable<PostModel> GetAllPosts(string sessionKey)
        {
            var context = new NewsFeedContext();
            using (context)
            {
                var posts =
                    (from post in context.Posts
                     where post.User.SessionKey == sessionKey
                     select new PostModel()
                     {
                         Title = post.Title,
                         Content = post.Content,
                         PublicDate = post.PublicDate,
                         Author = post.User.Username,
                         Avatar = post.User.Avatar
                     }).ToList();
                return posts;
            }
        }

        public static void CreatePost(string title, string content, string sessionKey)
        {
            using (NewsFeedContext context = new NewsFeedContext())
            {
                Post dbPost = new Post()
                {
                    Title = title,
                    Content = content,
                    PublicDate = DateTime.Now,
                    UserId = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey).Id
                };
                context.Posts.Add(dbPost);
                context.SaveChanges();
            }
        }

    }
}