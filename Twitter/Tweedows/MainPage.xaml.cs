using System;
using System.Collections.Generic;
using System.Linq;
using TweetSharp;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Navigation;
using HtmlAgilityPack;
using Tweetinvi;
using Twitter;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Tweedows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IAuthHelper iAuthHelper = new AuthHelper();

        public MainPage()
        {
            this.InitializeComponent();

        }

        private async void Launch_Click(object sender, RoutedEventArgs e)
        {
            progress.IsActive = true;
            twitterWebView.Source = new Uri(iAuthHelper.getUrl());
            this.Frame.Navigate(typeof(HomePage));
        }


        private async void twitterWebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            string html =
                await twitterWebView.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var pin = htmlDocument.DocumentNode.Descendants().FirstOrDefault(x => x.Id == "oauth.pin");

            if (pin != null)
            {
                if (iAuthHelper.GetAppCredentials() != null)
                {
                    var credentials = CredentialsCreator.GetCredentialsFromVerifierCode(pin.InnerHtml,
                        iAuthHelper.GetAppCredentials());
                    Auth.SetCredentials(credentials);
                }

                popupStackPanel.Visibility = Visibility.Collapsed;
            }
        }


        private void PopupClose(object sender, RoutedEventArgs e)
        {
            popupStackPanel.Visibility = Visibility.Collapsed;
        }
    }
}


