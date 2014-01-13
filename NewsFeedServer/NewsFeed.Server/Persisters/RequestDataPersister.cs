using NewsFeed.Data;
using NewsFeed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsFeed.Server.Persisters
{
    public class RequestDataPersister : BaseDataPersister
    {
        public static void SendRequest(string username, string sessionKey)
        {
            using (NewsFeedContext context = new NewsFeedContext())
            {
                Request dbRequest = new Request()
                {
                    RecipientId = (from user in context.Users
                                   where user.Username == username
                                   select user.Id).FirstOrDefault(),
                    SenderId = (from user in context.Users
                                where user.SessionKey == sessionKey
                                select user.Id).FirstOrDefault(),
                    Title = "Friend request",
                };
                context.Requests.Add(dbRequest);
                context.SaveChanges();
            }
        }

        public static void DeleteRequest(int id)
        {
            using (NewsFeedContext context = new NewsFeedContext())
            {
                var request = context.Requests.Find(id);
                context.Requests.Remove(request);
                context.SaveChanges();
            }
        }

        //Accept request
        public static void AddFriend(int senderId, string sessionKey)
        {
            using (NewsFeedContext context = new NewsFeedContext())
            {
                //Add request sender as friend in request receiver's list
                Friend dbFriendSender = new Friend()
                {
                    Username = (from request in context.Requests
                                where request.SenderId == senderId
                                select request.Sender.Username).FirstOrDefault(),
                    Avatar = (from request in context.Requests
                              where request.SenderId == senderId
                              select request.Sender.Avatar).FirstOrDefault(),
                    UserId = (from user in context.Users
                              where user.SessionKey == sessionKey
                              select user.Id).FirstOrDefault()
                };

                //Add request receiver as friend in request sender's list
                Friend dbFriendReceiver = new Friend()
                {
                    Username = (from user in context.Users
                                where user.SessionKey == sessionKey
                                select user.Username).FirstOrDefault(),
                    Avatar = (from user in context.Users
                              where user.SessionKey == sessionKey
                              select user.Avatar).FirstOrDefault(),
                    UserId = (from request in context.Requests
                              where request.SenderId == senderId
                              select request.Sender.Id).FirstOrDefault()
                };
                context.Friends.Add(dbFriendSender);
                context.Friends.Add(dbFriendReceiver);
                context.SaveChanges();
            }
        }
    }
}