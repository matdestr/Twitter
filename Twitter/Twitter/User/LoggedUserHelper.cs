using System.Collections.Generic;
using Tweetinvi;
using Tweetinvi.Core.Interfaces;

namespace Twitter
{
    public class LoggedUserHelper: ILoggedUserHelper
    {

        public IEnumerable<ITweet> GetHomeTimeline()
        {
            var loggedUser = User.GetLoggedUser();
            var homeTimeLine = loggedUser.GetHomeTimeline();
            return homeTimeLine; 
        }

        public IEnumerable<IMention> GetMentionsTimeline()
        {
            var loggedUser = User.GetLoggedUser();
            var mentionsTimeline = loggedUser.GetMentionsTimeline();
            return mentionsTimeline;
        }

        public IEnumerable<ITweet> GetUserTimeline()
        {
            var loggedUser = User.GetLoggedUser();
            var userTimeline = loggedUser.GetUserTimeline();
            return userTimeline;
        }
    }
}
