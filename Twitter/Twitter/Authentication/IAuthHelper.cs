using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi.Core.Credentials;

namespace Twitter
{
    interface IAuthHelper
    {
        TwitterCredentials GetAppCredentials();
        string getUrl();
    }
}
