using CustomServiceTestUtil;
using Microsoft.Identity.Client;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuthenticationUtility
{
    public class OAuthHelper
    {
        /// <summary>
        /// The header to use for OAuth.
        /// </summary>
        public const string OAuthHeader = "Authorization";

        public static string authorizationHeader;
        public static AuthenticationResult AuthenticationResult { get; private set; }
        /// <summary>
        /// Get a valid authentication header
        /// </summary>
        /// <returns>AuthenticationHeaderValue object</returns>
        public static AuthenticationHeaderValue GetValidAuthenticationHeader()
        {
            Task<string> token = AuthenticationHelper.GetAuthorizationHeader();
            string header = token.Result;
            return OAuthHelper.ParseAuthenticationHeader(header);
        }

        static AuthenticationHeaderValue ParseAuthenticationHeader(string authorizationHeader)
        {
            string[] split = authorizationHeader.Split(' ');
            string scheme = split[0];
            string parameter = split[1];
            return new AuthenticationHeaderValue(scheme, parameter);
        }


        /// <summary>
        /// Retrieves an authentication header from the service.
        /// </summary>
        /// <returns>The authentication header for the Web API call.</returns>
        public static async Task<string> GetAuthenticationHeader()
        {
            ServerSettings serverSetting = Settings.GetServerSettings();

            string aadTenant = string.Format("{0}/{1}", serverSetting.AzureAuthEndpoint, serverSetting.AADTenant);
            string aadResource = serverSetting.Ax7Endpoint;

            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(serverSetting.WebAppId)
            .WithClientSecret(serverSetting.WebAADKey)
            .WithAuthority(new Uri(aadTenant))
            .Build();

            string[] scopes = new string[] { $"{aadResource}/.default" };

            AuthenticationResult result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.CreateAuthorizationHeader();
        }
    }
}
