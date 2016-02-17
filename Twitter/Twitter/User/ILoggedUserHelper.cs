using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi.Core.Interfaces;

namespace Twitter
{
    interface ILoggedUserHelper
    {
        IEnumerable<ITweet> GetHomeTimeline();

        IEnumerable<IMention> GetMentionsTimeline();
        IEnumerable<ITweet> GetUserTimeline();

    }
}
