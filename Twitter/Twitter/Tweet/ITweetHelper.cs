using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.Models;

namespace Twitter
{
    interface ITweetHelper
    {
        ITweet PublishTweet(string text);
        ITweet PublishReplyTweet(string text, long tweetIdtoReplyTo);
        int GetTweetLength(string text);
        ITweet Retweet(long tweetId);
        bool DeleteTweet(long tweetId);
        bool FavouriteTweet(long tweetId);
        IEnumerable<ITweet> Search(string text);
    }
}
