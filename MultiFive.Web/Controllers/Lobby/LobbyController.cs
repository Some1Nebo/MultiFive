using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using MultiFive.Web.Infra;

namespace MultiFive.Web.Controllers.Lobby
{
    // OAuth code from:
    // https://github.com/DotNetOpenAuth/DotNetOpenAuth/wiki/Security-scenarios#authentication-and-authorization-via-google

    public class LobbyController : Controller
    {
        private static readonly WebServerClient AuthClient = AuthHelper.CreateClient();

        //
        // GET: /Lobby/

        public ActionResult Index()
        {
            return string.IsNullOrEmpty(Request.QueryString["code"])
                ? InitAuth()
                : OAuthCallback();
        }

        private ActionResult InitAuth()
        {
            var state = new AuthorizationState();
            string uri = RemoveQueryStringFromUri(Request.Url.AbsoluteUri);
            state.Callback = new Uri(uri);

            state.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
            //state.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
            //state.Scope.Add("https://www.googleapis.com/auth/calendar");

            var r = AuthClient.PrepareRequestUserAuthorization(state);
            return r.AsActionResultMvc5();
        }

        private ActionResult OAuthCallback()
        {
            var auth = (Session["auth"] as IAuthorizationState) ?? AuthClient.ProcessUserAuthorization(this.Request);
            Session["auth"] = auth;

            var google = new GoogleProxy();

            // TODO: Validate the Token Information;

            dynamic userInfo = google.GetUserInfo(auth.AccessToken);

            /*
             * Format:
             {
              "id": "xxx",
              "name": "Tagir Magomedov",
              "given_name": "Tagir",
              "family_name": "Magomedov",
              "link": "xxx",
              "picture": "https://xxx/photo.jpg",
              "gender": "male",
              "locale": "en"
            }*/

            // Later, if necessary:
            // bool success = client.RefreshAuthorization(auth);
            return View(userInfo);
        }

        private static string RemoveQueryStringFromUri(string uri)
        {
            int index = uri.IndexOf('?');

            if (index > -1)
            {
                uri = uri.Substring(0, index);
            }

            return uri;
        }
    }

    public class GoogleProxy
    {
        public dynamic GetUserInfo(string authToken)
        {
            var userInfoUrl = "https://www.googleapis.com/oauth2/v1/userinfo";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            var response = httpClient.GetAsync(userInfoUrl).Result;
            dynamic userInfo = response.Content.ReadAsAsync<dynamic>().Result;
            return userInfo;
        }

        public dynamic GetTokenInfo(string accessToken)
        {
            var verificationUri = "https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=" + accessToken;

            var httpClient = new HttpClient();

            var response = httpClient.GetAsync(verificationUri).Result;
            dynamic tokenInfo = response.Content.ReadAsAsync<dynamic>().Result;
            return tokenInfo;
        }
    }
}