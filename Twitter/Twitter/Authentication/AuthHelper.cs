using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi;
using Tweetinvi.Core.Credentials;

namespace Twitter
{
    public class AuthHelper : IAuthHelper
    {
        private TwitterCredentials appCredentials = new TwitterCredentials("uQfTVDObrFmQ59QcFOtjQ66K8", "P4wgq79ULgTM12o1ImrR9IowIG0E49R5MN4vCy492aGPkZ17q9");

        public TwitterCredentials GetAppCredentials()
        {
            return appCredentials;
        }

        public string getUrl()
        {
            var url = CredentialsCreator.GetAuthorizationURL(appCredentials);
            return url;

        }


    }
}
