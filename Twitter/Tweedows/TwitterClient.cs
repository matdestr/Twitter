using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace Tweedows
{
    public class TwitterClient
    {

        public class TwitterCallbackUrl
        {
            public static string Text { get { return "http://somecallbackaddresshere.com/blah.html"; } }
        }

        public class TwitterClientID
        {
            public static string Text { get { return "WYc94TiVFdI9ZE89MjWaOv01B"; } }
        }

        public class TwitterClientSecret
        {
            public static string Text { get { return "NRdckPoH2eAQikLrlUFdHLYhISFjGzaYHnXFcjmoKg1jkMfJpe"; } }
        }

        public class TwitterReturnedToken
        {
            public static string Text { get; set; }
        }

        private string screen_name { get; set; }
        private string o_auth_token_secret { get; set; }
        private string o_auth_token { get; set; }
        private readonly IDictionary<string, string> customParameters = new Dictionary<string, string>();


        public string GetNonce()
        {
            /*
            Random rand = new Random();
            int nonce = rand.Next(1000000000);
            return nonce.ToString();*/
           return Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
        }

        public string GetTimeStamp()
        {
            TimeSpan SinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Math.Round(SinceEpoch.TotalSeconds).ToString();
        }

     

        public async Task<string> GetTwitterRequestTokenAsync(string twitterCallbackUrl, string consumerKey)
        {
            // 
            // Acquiring a request token 
            // 
            string TwitterUrl = "https://api.twitter.com/oauth/request_token";

            string nonce = GetNonce();
            string timeStamp = GetTimeStamp();
            string SigBaseStringParams = "oauth_callback=" + Uri.EscapeDataString(twitterCallbackUrl);
            SigBaseStringParams += "&" + "oauth_consumer_key=" + consumerKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            string SigBaseString = "GET&";
            SigBaseString += Uri.EscapeDataString(TwitterUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);
            string Signature = GetSignature(SigBaseString);

            TwitterUrl += "?" + SigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(Signature);
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            string GetResponse = await httpClient.GetStringAsync(new Uri(TwitterUrl));

            //          DebugPrint("Received Data: " + GetResponse); 

            string request_token = null;
            string oauth_token_secret = null;
            string[] keyValPairs = GetResponse.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                string[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }
            o_auth_token = request_token;
            DebugPrint(o_auth_token);
            return request_token;
        }

        public async void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            WebAuthenticationResult result = args.WebAuthenticationResult;


            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                OutputToken(result.ResponseData.ToString());
                await GetOAuthToken(result.ResponseData.ToString());
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                OutputToken("HTTP Error returned by AuthenticateAsync() : " + result.ResponseErrorDetail.ToString());
            }
            else
            {
                OutputToken("Error returned by AuthenticateAsync() : " + result.ResponseStatus.ToString());
            }
        }





        private async Task<String> SendDataAsync(String Url)
        {
            try
            {
                Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
                return await httpClient.GetStringAsync(new Uri(Url));
            }
            catch (Exception Err)
            {
                //rootpage.NotifyUser("Error getting data from server." + Err.Message, NotifyType.StatusMessage);
            }

            return null;
        }

        private void DebugPrint(String Trace)
        {
            //TwitterDebugArea.Text += Trace + "\r\n";
        }

        private void OutputToken(String TokenUri)
        {
            TwitterClient.TwitterReturnedToken.Text = TokenUri;
            o_auth_token = TokenUri;
        }

        public async Task GetOAuthToken(string webAuthResultResponseData)
        {
            // 
            // Acquiring a access_token first 
            // 

            string responseData = webAuthResultResponseData.Substring(webAuthResultResponseData.IndexOf("oauth_token"));
            string request_token = null;
            string oauth_verifier = null;
            String[] keyValPairs = responseData.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                String[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_verifier":
                        oauth_verifier = splits[1];
                        break;
                }
            }

            String TwitterUrl = "https://api.twitter.com/oauth/access_token";

            string timeStamp = GetTimeStamp();
            string nonce = GetNonce();

            string SigBaseStringParams = "oauth_consumer_key=" + TwitterClient.TwitterClientID.Text;
            SigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            SigBaseStringParams += "&" + "oauth_token=" + request_token;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            String SigBaseString = "POST&";
            SigBaseString += Uri.EscapeDataString(TwitterUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);

            String Signature = GetSignature(SigBaseString);

            HttpStringContent httpContent = new HttpStringContent("oauth_verifier=" + oauth_verifier, 
                Windows.Storage.Streams.UnicodeEncoding.Utf8);
            httpContent.Headers.ContentType = HttpMediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            string authorizationHeaderParams = "oauth_consumer_key=\"" + TwitterClient.TwitterClientID.Text + 
                "\", oauth_nonce=\"" + nonce + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"" 
                + Uri.EscapeDataString(Signature) + "\", oauth_timestamp=\"" + timeStamp + "\", oauth_token=\"" 
                + Uri.EscapeDataString(request_token) + "\", oauth_version=\"1.0\"";

            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", authorizationHeaderParams);
            var httpResponseMessage = await httpClient.PostAsync(new Uri(TwitterUrl), httpContent);
            string response = await httpResponseMessage.Content.ReadAsStringAsync();

            String[] Tokens = response.Split('&');
            string oauth_token_secret = null;
            string access_token = null;
            string screen_name = null;

            for (int i = 0; i < Tokens.Length; i++)
            {
                String[] splits = Tokens[i].Split('=');
                switch (splits[0])
                {
                    case "screen_name":
                        screen_name = splits[1];
                        break;
                    case "oauth_token":
                        access_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }

                
            }


            //you can store access_token and oauth_token_secret for further use. See Scenario5(Account Management). 
            if (access_token != null)
            {
                DebugPrint("access_token = " + access_token);
            }

            if (oauth_token_secret != null)
            {
                DebugPrint("oauth_token_secret = " + oauth_token_secret);
            }
            if (screen_name != null)
            {
                this.screen_name = screen_name;
            }

            this.o_auth_token = access_token;
            this.o_auth_token_secret = o_auth_token;
        }
        /*
        private string GetSignatureForGetRequest(string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var dataToSign = new StringBuilder()
                .Append("GET").Append("&")
                .Append(Helper.EncodeRFC3986(url)).Append("&")
                .Append(parameters
                    .OrderBy(x => x.Key)
                    .Select(x => string.Format("{0}={1}", x.Key, x.Value))
                    .Join("&")
                    .EncodeRFC3986());
           
            return GetSignature(dataToSign.ToString());
        }
        */

        public string GetSignature(string sigBaseString)
        {
            IBuffer KeyMaterial = CryptographicBuffer.ConvertStringToBinary(TwitterClientSecret.Text + "&", BinaryStringEncoding.Utf8);
            MacAlgorithmProvider HmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey MacKey = HmacSha1Provider.CreateKey(KeyMaterial);
            IBuffer DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8);
            IBuffer SignatureBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);
            string Signature = CryptographicBuffer.EncodeToBase64String(SignatureBuffer);

            return Signature;
        }
        

        public virtual Task<IEnumerable<Tweet>> GetHomeTimelineAsync(long? sinceId = null, int count = 20)
        {
            return GetListAsync("https://api.twitter.com/1.1/statuses/home_timeline.json", sinceId, count);
        }

      

        private string GenerateAuthorizationHeaderValue(IEnumerable<KeyValuePair<string, string>> parameters, string signature)
        {
            IEnumerable<KeyValuePair<string, string>> test = new[] { new KeyValuePair<string, string>("oauth_signature", signature) };
            return new StringBuilder("OAuth ")
                .Append(parameters.Concat(test)
                            .Where(x => x.Key.StartsWith("oauth_"))
                            .Select(x => string.Format("{0}=\"{1}\"", x.Key, x.Value.EncodeRFC3986()))
                            .Join(","))
                .ToString();
        }


        private void AddOAuthParameters(IDictionary<string, string> parameters, string timestamp, string nonce)
        {
            parameters.Add("oauth_consumer_key", TwitterClient.TwitterClientID.Text);
            parameters.Add("oauth_nonce", nonce);
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            parameters.Add("oauth_timestamp", timestamp);
            parameters.Add("oauth_token", o_auth_token);
            parameters.Add("oauth_version", "1.0");
        }

        /*
        protected async Task<IEnumerable<T>> GetIEnumerableAsync<T>(string url, Dictionary<string, string> parameters)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
            //var parameters = new Dictionary<string, string>(customParameters);

            AddOAuthParameters(parameters, GetTimeStamp(), GetNonce());

            var signature = GetSignatureForGetRequest(url, parameters);
            var headerValue = GenerateAuthorizationHeaderValue(parameters, signature);

            httpClient.DefaultRequestHeaders.Add("Authorization", headerValue);

            List<T> result = new List<T>();

            try
            {
                var firstResponse = await httpClient.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(GetRequestUrl(url, parameters))));

                var responseContent = await firstResponse.Content.ReadAsStringAsync();

                var resultingList = JToken.Parse(responseContent);

                await Task.Run(() =>
                {
                    foreach (var item in resultingList.AsJEnumerable())
                    {
                        result.Add(item.ToObject<T>());
                    }
                });
            }
            catch
            {
                // Empty list
            }

            return result;
        }
    */

        private string GetRequestUrl(string url, Dictionary<string, string> parameters)
        {
            return string.Format("{0}?{1}", url, GetCustomParametersString(parameters));
        }

        /*
        private Task<IEnumerable<Tweet>> GetListAsync(string url, long? sinceId, int? count)
        {
            var dictionary = new Dictionary<string, string>();

            if (sinceId.HasValue)
                dictionary.Add("since_id", sinceId.Value.ToString());

            if (count.HasValue)
                dictionary.Add("count", count.Value.ToString());

            return GetIEnumerableAsync<Tweet>(url, dictionary);
        }*/

        private string GetCustomParametersString(Dictionary<string, string> parameters)
        {
            return parameters.Select(x => string.Format("{0}={1}", x.Key, x.Value)).Join("&");
        }

        public async Task GetAccessToken(string requestToken, string oauthVerifier)
        {
            String TwitterUrl = "https://api.twitter.com/oauth/access_token";

            string timeStamp = GetTimeStamp();
            string nonce = GetNonce();

            String SigBaseStringParams = "oauth_consumer_key=" +TwitterClientID.Text;
            SigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            SigBaseStringParams += "&" + "oauth_token=" + requestToken;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            String SigBaseString = "POST&";

            SigBaseString += Uri.EscapeDataString(TwitterUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);

            String Signature = GetSignature(SigBaseString);

            HttpStringContent httpContent = new HttpStringContent("oauth_verifier=" + oauthVerifier,
                Windows.Storage.Streams.UnicodeEncoding.Utf8);
            httpContent.Headers.ContentType = HttpMediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            string authorizationHeaderParams = "oauth_consumer_key=\"" + TwitterClientID.Text + "\", oauth_nonce=\"" + nonce +
                                               "\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"" +
                                               Uri.EscapeDataString(Signature) + "\", oauth_timestamp=\"" + timeStamp +
                                               "\", oauth_token=\"" + Uri.EscapeDataString(requestToken) +
                                               "\", oauth_version=\"1.0\"";
            

            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", authorizationHeaderParams);
            var httpResponseMessage = await httpClient.PostAsync(new Uri(TwitterUrl), httpContent);
            string response = await httpResponseMessage.Content.ReadAsStringAsync();

            String[] Tokens = response.Split('&');
            string oauth_token_secret = null;
            string oauth_token = null;
            string screen_name = null;

            for (int i = 0; i < Tokens.Length; i++)
            {
                String[] splits = Tokens[i].Split('=');
                switch (splits[0])
                {
                    case "screen_name":
                        screen_name = splits[1];
                        break;
                    case "oauth_token":
                        oauth_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }

            if (oauth_token != null)
            {
                //App.SettingsStore.TwitteroAuthToken = oauth_token;
            }

            if (oauth_token_secret != null)
            {
                //App.SettingsStore.TwitteroAuthTokenSecret = oauth_token_secret;
            }
            if (screen_name != null)
            {
                //App.SettingsStore.TwitterName = screen_name;
            }

            this.o_auth_token_secret = oauth_token_secret;
            this.o_auth_token = oauth_token;
            this.screen_name = screen_name;
        }



        public async void Authenticate_Twitter() {
            try
            {
                
                string oauth_token = await GetTwitterRequestTokenAsync(TwitterClient.TwitterCallbackUrl.Text, TwitterClient.TwitterClientID.Text);
                o_auth_token = oauth_token;
                string TwitterUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" + oauth_token;
                System.Uri StartUri = new Uri(TwitterUrl);
                System.Uri EndUri = new Uri(TwitterClient.TwitterCallbackUrl.Text);

                //                DebugPrint("Navigating to: " + TwitterUrl); 

                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, StartUri, EndUri);
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    OutputToken(WebAuthenticationResult.ResponseData.ToString());
                    await GetOAuthToken(WebAuthenticationResult.ResponseData.ToString());
                }
                else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    OutputToken("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
                }
                else
                {
                    OutputToken("Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString());
                }
            }
            catch (Exception Error)
            {
                // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                //rootPage.NotifyUser(Error.Message, NotifyType.ErrorMessage);

            }
        }


    }
}
