using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Windows.UI.Xaml.Controls;

using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

using System.Net.Http.Headers;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Newtonsoft.Json;
using System.ComponentModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tweedows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public class TweetViewModel
        {
            public ObservableCollection<Tweet> tweets = new ObservableCollection<Tweet>();
            public ObservableCollection<Tweet> Tweets { get { return this.tweets; } }
        }

        public class MentionTweetViewModel
        {
            public ObservableCollection<Tweet> tweets = new ObservableCollection<Tweet>();
            public ObservableCollection<Tweet> Tweets { get { return this.tweets; } }
        }

        public class ProfileTweetViewModel
        {
            public ObservableCollection<Tweet> tweets = new ObservableCollection<Tweet>();
            public ObservableCollection<Tweet> Tweets { get { return this.tweets; } }
        }
        public HomePage()
        {
            this.InitializeComponent();
            this.ViewModel = new TweetViewModel();
            this.MentionsViewModel = new MentionTweetViewModel();
            this.ProfileViewModel = new ProfileTweetViewModel();
        }

        public TweetViewModel ViewModel { get; set; }
        public MentionTweetViewModel MentionsViewModel { get; set; }
        public ProfileTweetViewModel ProfileViewModel { get; set; }

        private TwitterService TwitterClient { get; set; }
        string _getHomeTimelineUrl = "https://api.twitter.com/1.1/statuses/home_timeline.json";
        string _getMentionsUrl = "https://api.twitter.com/1.1/statuses/mentions_timeline.json";
        string _getUserTimelineUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json";

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is TwitterService)
            {
                TwitterClient = e.Parameter as TwitterService;
                List<Tweet> lists = await TwitterClient.requestTwitterApi(_getHomeTimelineUrl);
                foreach (Tweet tweet in lists)
                {
                    ViewModel.tweets.Add(tweet);
                }
                List<Tweet> mentions = await TwitterClient.requestTwitterApi(_getMentionsUrl);
                foreach (Tweet tweet in mentions)
                {
                    MentionsViewModel.tweets.Add(tweet);
                }
                List<Tweet> userstweets = await TwitterClient.requestTwitterApi(_getUserTimelineUrl);
                foreach (Tweet tweet in userstweets)
                {
                    ProfileViewModel.tweets.Add(tweet);
                }
            }
            else
            {
                //no tweets
            }
            base.OnNavigatedTo(e);
        }

        private void AppBarToggleButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewTweet), TwitterClient);

        }

        private async void RetweetButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Tweet tweet = ((Button)sender).Tag as Tweet;
            if (tweet.retweeted)
            {
                tweet.retweeted = ! await TwitterClient.Unretweet(tweet.id);
                renewTweet(tweet);
            }
            else {
                tweet.retweeted = await TwitterClient.Retweet(tweet.id);
                renewTweet(tweet);
            }
        }

        private async void LikeButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Tweet tweet = ((Button)sender).Tag as Tweet;
            if (tweet.favorited)
            {
                tweet.favorited = !await TwitterClient.Unfavorite(tweet.id);
                renewTweet(tweet);
            }
            else {
                tweet.favorited = await TwitterClient.Favorite(tweet.id);
                renewTweet(tweet);
            }
        }

        private void renewTweet(Tweet newTweet) {
            var originalTweet = ViewModel.tweets.First(t => t.id == newTweet.id);
            var index = ViewModel.tweets.IndexOf(originalTweet);
            ViewModel.tweets.Remove(originalTweet);
            ViewModel.tweets.Insert(index, newTweet);
        }


    }
}