using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.Models;

namespace Twitter
{
    public class TweetHelper : ITweetHelper
    {
        public ITweet PublishTweet(string text)
        {
            var publishedTweet = Tweet.PublishTweet(text);
            return publishedTweet;
        }

        public ITweet PublishReplyTweet(string text, long tweetIdtoReplyTo)
        {
            var tweetToReplyTo = Tweet.GetTweet(tweetIdtoReplyTo);
            var textToPublish = string.Format("@{0} {1}", tweetToReplyTo.CreatedBy.ScreenName, text);
            var publishedTweet = Tweet.PublishTweetInReplyTo(textToPublish, tweetIdtoReplyTo);
            return publishedTweet;
        }

        public int GetTweetLength(string text)
        {
            return text.TweetLength(); 
        }

        public ITweet Retweet(long tweetId)
        {
            var retweet = Tweet.PublishRetweet(tweetId);
            return retweet;
        }

        public bool DeleteTweet(long tweetId)
        {
            var success = Tweet.DestroyTweet(tweetId);
            return success;
        }

        public bool FavouriteTweet(long tweetId)
        {
            var success = Tweet.FavoriteTweet(tweetId);
            return success;
        }

        public IEnumerable<ITweet> Search(string text)
        {
            var matchingTweets = Tweetinvi.Search.SearchTweets(text);
            return matchingTweets;
        }
    }
}
