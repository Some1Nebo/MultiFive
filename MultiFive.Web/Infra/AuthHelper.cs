using System;
using System.Configuration;
using DotNetOpenAuth.OAuth2;

namespace MultiFive.Web.Infra
{
    // taken from https://github.com/DotNetOpenAuth/DotNetOpenAuth/wiki/Security-scenarios#authentication-and-authorization-via-google

    public class AuthHelper
    {
        private const string ClientId = "184518635745.apps.googleusercontent.com";
        private const string ClientSecret = "jo3mavbGpvE197eyBH7AJVgP";

        public static WebServerClient CreateClient()
        {
            var desc = GetAuthServerDescription();

            var client = new WebServerClient(desc, ClientId)
            {
                ClientCredentialApplicator = ClientCredentialApplicator.PostParameter(ClientSecret)
            };

            return client;
        }

        public static AuthorizationServerDescription GetAuthServerDescription()
        {
            var authServerDescription = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(@"https://accounts.google.com/o/oauth2/auth"),
                TokenEndpoint = new Uri(@"https://accounts.google.com/o/oauth2/token"),
                ProtocolVersion = ProtocolVersion.V20
            };

            return authServerDescription;
        }
    }
}
