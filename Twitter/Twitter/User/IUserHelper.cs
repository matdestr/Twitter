using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi.Core.Interfaces;

namespace Twitter
{
    interface IUserHelper
    {
        IUser GetUserById(long userId);
        IUser GetUserByScreenName(string screenName);
        IEnumerable<IUser> SearchUser(string text);
    }
}
