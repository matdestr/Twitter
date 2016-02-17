using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi;
using Tweetinvi.Core.Interfaces;

namespace Twitter
{
    public class UserHelper:IUserHelper
    {
        public IUser GetUserById(long userId)
        {
            var user = User.GetUserFromId(userId);
            return user;
        }

        public IUser GetUserByScreenName(string screenName)
        {
            var user = User.GetUserFromScreenName(screenName);
            return user;
        }

        public IEnumerable<IUser> SearchUser(string text)
        {
            var users = Search.SearchUsers(text);
            return users;
        }
    }
}
