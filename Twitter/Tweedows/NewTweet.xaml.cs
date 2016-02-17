using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Twitter;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tweedows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewTweet : Page
    {

        private ITweetHelper tweetHelper = new TweetHelper();
        public NewTweet()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void RichEditBox_TextChanged(object sender, RoutedEventArgs e)
        {
            //TODO count character (max 140)
            string text;
            richEditBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out text);

            if (tweetHelper.GetTweetLength(text) <= 140)
            {
                //do nothing
            }
            else
            {
                //display message
                var dialog = new MessageDialog("Your tweet has more than 140 characters");
                await dialog.ShowAsync();
            }


        }



        private void appBarButton_Click(object sender, RoutedEventArgs e)
        {
            string text;
            richEditBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out text);
            tweetHelper.PublishTweet(text);
        }
    }
}
