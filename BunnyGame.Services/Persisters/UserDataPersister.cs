using BunnyGame.Data;
using BunnyGame.Models;
using BunnyGame.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyGame.Services.Persisters
{
    public class UserDataPersister : BaseDataPersister
    {
        private const string SessionKeyChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const int SessionKeyLen = 50;
        protected static Random rand = new Random();

        private const int Sha1CodeLength = 40;
        private const string ValidUsernameChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_1234567890";
        private const string ValidNicknameChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_1234567890 -";
        private const int MinUsernameNicknameChars = 6;
        private const int MaxUsernameNicknameChars = 30;

        /* private methods */

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
            if (username == null || username.Length < MinUsernameNicknameChars || username.Length > MaxUsernameNicknameChars)
            {
                throw new ServerErrorException(string.Format("Username should be between {0} and {1} symbols long", MinUsernameNicknameChars, MaxUsernameNicknameChars), "INV_USR_LEN");
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

        /* public methods */

        public static void CreateUser(string username, string authCode, string avatar)
        {
            ValidateUsername(username);
            ValidateAuthCode(authCode);
            using (BunnyGameContext context = new BunnyGameContext())
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
            var context = new BunnyGameContext();
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
        
        //public static void LogoutUser(string sessionKey)
        //{
        //    ValidateSessionKey(sessionKey);
        //    var context = new BunnyGameContext();
        //    using (context)
        //    {
        //        var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
        //        if (user == null)
        //        {
        //            throw new ServerErrorException("Invalid user authentication", "INV_USR_AUTH");
        //        }
        //        user.SessionKey = null;
        //        context.SaveChanges();
        //    }
        //}

        public static IEnumerable<PostModel> GetAllPosts(string sessionKey)
        {
            var context = new BunnyGameContext();
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
            using (BunnyGameContext context = new BunnyGameContext())
            {
                Post dbPost = new Post()
                {
                    Title=title,
                    Content = content,
                    PublicDate = DateTime.Now,
                    UserId = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey).Id
                };
                context.Posts.Add(dbPost);
                context.SaveChanges();
            }
        }

        public static UserSearchModel GetUser(string username)
        {
            var context = new BunnyGameContext();
            using (context)
            {
                var user =
                    (from u in context.Users
                     where u.Username == username
                     select new UserSearchModel()
                     {
                         Username = u.Username,
                         Avatar = u.Avatar
                     });
                return user.FirstOrDefault();
            }
        }

        public static UserLoggedModel GetUserBySessionKey(string sessionKey)
        {
            var context = new BunnyGameContext();
            using (context)
            {
                var user =
                    (from u in context.Users
                    where u.SessionKey == sessionKey
                    select new UserLoggedModel()
                    {
                        Username = u.Username,
                        Avatar = u.Avatar,
                        //Have to be here?
                        SessionKey = u.SessionKey,
                        Wall = (from post in context.Posts
                                //Add friends posts here
                                where post.UserId == u.Id
                                select new PostModel()
                                {
                                    Title = post.Title,
                                    Content = post.Content,
                                    PublicDate = post.PublicDate,
                                    Author = post.User.Username,
                                    Avatar = post.User.Avatar
                                }).ToList(),
                        ReceiveRequests = (from request in context.Requests
                                            where request.RecipientId == u.Id
                                            select new ReceiveRequestModel()
                                            {
                                               Id = request.Id,
                                               Title = request.Title,
                                               SenderId = request.SenderId,
                                               SenderName = request.Sender.Username,
                                               SenderAvatar = request.Sender.Avatar,
                                               Answer = request.Answer,
                                            }).ToList(),
                        Friends = (from friend in context.Friends
                                   where friend.UserId == u.Id
                                   select new UserFriendModel()
                                   {
                                       Username = friend.Username,
                                       Avatar = friend.Avatar,
                                       Wall = (from post in context.Posts
                                               //Filter post friend with username - username it's unique
                                               where post.User.Username==friend.Username 
                                               select new PostModel()
                                               {
                                                   Title = post.Title,
                                                   Content = post.Content,
                                                   PublicDate = post.PublicDate,
                                                   Author = post.User.Username,
                                                   Avatar = post.User.Avatar
                                               }).ToList()
                                   }).ToList()
                    });
                return user.FirstOrDefault();
            }
        }

        public static void AddFriend(int senderId, string sessionKey)
        {
            using (BunnyGameContext context = new BunnyGameContext())
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