using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HtmlAgilityPack;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Credentials;
using CredentialsCreator = Tweetinvi.CredentialsCreator;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Twitter.WinPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IAuthHelper authHelper = new AuthHelper();
        private ITweetHelper tweetHelper = new TweetHelper();


        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
            
           
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            twitterWebView.Source = new Uri(authHelper.getUrl());
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tweetTextBox.Text))
            {
                tweetHelper.PublishTweet(tweetTextBox.Text);
                tweetTextBox.Text = "";
            }
        }

        private void PopupClose(object sender, RoutedEventArgs e)
        {
            popupStackPanel.Visibility = Visibility.Collapsed;
        }

        private async void twitterWebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            string html =
                await twitterWebView.InvokeScriptAsync("eval", new string[] {"document.documentElement.outerHTML;"});
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var pin = htmlDocument.DocumentNode.Descendants().FirstOrDefault(x => x.Id == "oauth.pin");

            if (pin != null)
            {
                if (authHelper.GetAppCredentials() != null)
                {
                    var credentials = CredentialsCreator.GetCredentialsFromVerifierCode(pin.InnerHtml, authHelper.GetAppCredentials());
                    Auth.SetCredentials(credentials);
                }

                popupStackPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
