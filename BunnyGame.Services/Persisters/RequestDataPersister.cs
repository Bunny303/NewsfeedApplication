using BunnyGame.Data;
using BunnyGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BunnyGame.Services.Persisters
{
    public class RequestDataPersister : BaseDataPersister
    {
        public static void AddRequest(string username, string avatar, string sessionKey)
        {
            using (BunnyGameContext context = new BunnyGameContext())
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
                    Answer = false
                };
                context.Requests.Add(dbRequest);
                context.SaveChanges();
            }
        }

        public static void DeleteRequest(int id)
        {
            using (BunnyGameContext context = new BunnyGameContext())
            {
                var request = context.Requests.Find(id);
                context.Requests.Remove(request);
                context.SaveChanges();
            }
        }
    }
}