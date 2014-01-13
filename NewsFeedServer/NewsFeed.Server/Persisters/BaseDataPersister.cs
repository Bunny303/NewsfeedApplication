using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsFeed.Server.Models;
using NewsFeed.Models;
using NewsFeed.Data;

namespace NewsFeed.Server.Persisters
{
    public class BaseDataPersister
    {
        protected const int Sha1CodeLength = 40;
        
        protected static User GetUser(int userId, NewsFeedContext context)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new ServerErrorException("Invalid user", "ERR_INV_USR");
            }
            return user;
        }
    }
}