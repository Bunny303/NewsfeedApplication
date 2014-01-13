using NewsFeed.Data;
using NewsFeed.Models;
using NewsFeed.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFeed.Server.Persisters
{
    public class UserDataPersister : BaseDataPersister
    {
        private const string SessionKeyChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const int SessionKeyLen = 50;
        protected static Random rand = new Random();

        private const int Sha1CodeLength = 40;
        private const string ValidUsernameChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_1234567890";
        private const int MinUsernameChars = 6;
        private const int MaxUsernameChars = 30;

        private static void ValidateSessionKey(string sessionKey)
        {
            if (sessionKey.Length != SessionKeyLen || sessionKey.Any(ch => !SessionKeyChars.Contains(ch)))
            {
                throw new ServerErrorException("Invalid Password", "ERR_INV_AUTH");
            }
        }

        private static string GenerateSessionKey(int userId)
        {
            StringBuilder keyChars = new StringBuilder(50);
            keyChars.Append(userId.ToString());
            while (keyChars.Length < SessionKeyLen)
            {
                int randomCharNum;
                lock (rand)
                {
                    randomCharNum = rand.Next(SessionKeyChars.Length);
                }
                char randomKeyChar = SessionKeyChars[randomCharNum];
                keyChars.Append(randomKeyChar);
            }
            string sessionKey = keyChars.ToString();
            return sessionKey;
        }

        private static void ValidateUsername(string username)
        {
            if (username == null || username.Length < MinUsernameChars || username.Length > MaxUsernameChars)
            {
                throw new ServerErrorException(string.Format("Username should be between {0} and {1} symbols long", MinUsernameChars, MaxUsernameChars), "INV_USR_LEN");
            }
            else if (username.Any(ch => !ValidUsernameChars.Contains(ch)))
            {
                throw new ServerErrorException("Username contains invalid characters", "INV_USR_CHARS");
            }
        }
        
        private static void ValidateAuthCode(string authCode)
        {
            if (authCode.Length != Sha1CodeLength)
            {
                throw new ServerErrorException("Invalid user authentication", "INV_USR_AUTH");
            }
        }

        public static void CreateUser(string username, string authCode, string avatar)
        {
            ValidateUsername(username);
            ValidateAuthCode(authCode);
            using (NewsFeedContext context = new NewsFeedContext())
            {
                var usernameToLower = username.ToLower();

                var dbUser = context.Users.FirstOrDefault(u => u.Username == usernameToLower);

                if (dbUser != null)
                {
                    throw new ServerErrorException("Username already exists", "ERR_DUP_USR");
                }

                dbUser = new User()
                {
                    Username = usernameToLower,
                    AuthenticationCode = authCode,
                    Avatar = avatar
                };
                context.Users.Add(dbUser);
                context.SaveChanges();
            }
        }

        public static string LoginUser(string username, string authCode, out string avatar)
        {
            ValidateUsername(username);
            ValidateAuthCode(authCode);
            var context = new NewsFeedContext();
            using (context)
            {
                var usernameToLower = username.ToLower();
                var user = context.Users.FirstOrDefault(u => u.Username == usernameToLower && u.AuthenticationCode == authCode);
                if (user == null)
                {
                    throw new ServerErrorException("Invalid username or password", "ERR_INV_USR");
                }

                var sessionKey = GenerateSessionKey((int)user.Id);
                user.SessionKey = sessionKey;
                avatar = user.Avatar;
                context.SaveChanges();
                return sessionKey;
            }
        }

        public static IEnumerable<UserSearchModel> GetUser(string username)
        {
            var context = new NewsFeedContext();
            using (context)
            {
                var users =
                    (from u in context.Users
                     where u.Username.Contains(username)
                     select new UserSearchModel()
                     {
                         Username = u.Username,
                         Avatar = u.Avatar
                     }).ToList();
                return users;
            }
        }

        public static UserLoggedModel GetUserBySessionKey(string sessionKey)
        {
            ValidateSessionKey(sessionKey);
            var context = new NewsFeedContext();
            using (context)
            {
                var user =
                    from u in context.Users
                    where u.SessionKey == sessionKey
                    select new UserLoggedModel()
                    {
                        Username = u.Username,
                        Avatar = u.Avatar,
                        SessionKey = sessionKey
                    };
                return user.FirstOrDefault();
            }
        }

        public static IEnumerable<UserFriendModel> GetUserFriendsBySessionKey(string sessionKey)
        {
            ValidateSessionKey(sessionKey);
            var context = new NewsFeedContext();
            using (context)
            {
                int userId = context.Users.Where(u => u.SessionKey == sessionKey)
                    .Select(u => u.Id).FirstOrDefault();

                var friends =
                    (from friend in context.Friends
                     where friend.UserId == userId
                     select new UserFriendModel()
                     {
                         Username = friend.Username,
                         Avatar = friend.Avatar,
                         Wall = (from post in context.Posts
                                 //Filter post friend with username - username it's unique
                                 where post.User.Username == friend.Username
                                 select new PostModel()
                                 {
                                     Title = post.Title,
                                     Content = post.Content,
                                     PublicDate = post.PublicDate,
                                     Author = post.User.Username,
                                     Avatar = post.User.Avatar
                                 }).ToList()
                     }).ToList();
                    
                return friends;
            }
        }

        public static IEnumerable<ReceiveRequestModel> GetUserRequestsBySessionKey(string sessionKey)
        {
            ValidateSessionKey(sessionKey);
            var context = new NewsFeedContext();
            using (context)
            {
                int userId = context.Users.Where(u => u.SessionKey == sessionKey)
                    .Select(u => u.Id).FirstOrDefault();

                var requests =
                    (from request in context.Requests
                     where request.RecipientId == userId
                     select new ReceiveRequestModel()
                     {
                         Id = request.Id,
                         Title = request.Title,
                         SenderId = request.SenderId,
                         SenderName = request.Sender.Username,
                         SenderAvatar = request.Sender.Avatar,
                     }).ToList();

                return requests;
            }
        }

        public static IEnumerable<PostModel> GetUserWallBySessionKey(string sessionKey)
        {
            ValidateSessionKey(sessionKey);
            var context = new NewsFeedContext();
            using (context)
            {
                int userId = context.Users.Where(u => u.SessionKey == sessionKey)
                    .Select(u => u.Id).FirstOrDefault();

                var wall =
                    (from post in context.Posts
                     //Add friends posts here
                     where post.UserId == userId
                     select new PostModel()
                     {
                         Title = post.Title,
                         Content = post.Content,
                         PublicDate = post.PublicDate,
                         Author = post.User.Username,
                         Avatar = post.User.Avatar
                     }).ToList();

                return wall;
            }
        }
    }
}