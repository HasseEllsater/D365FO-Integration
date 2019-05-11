using CustomServiceTestUtil;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http.Headers;
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
            return OAuthHelper.ParseAuthenticationHeader(AuthenticationHelper.AuthorizationHeader);
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
        public static string GetAuthenticationHeader()
        {
            ServerSettings serverSetting = Settings.GetServerSettings();

            string aadTenant = string.Format("{0}/{1}", serverSetting.AzureAuthEndpoint, serverSetting.AADTenant);
            string aadResource = serverSetting.Ax7Endpoint;

            AuthenticationContext authenticationContext = new AuthenticationContext(aadTenant);
            AuthenticationResult authenticationResult = null;

            try
            {
                var credential = new ClientCredential(serverSetting.WebAppId, serverSetting.WebAADKey);
                authenticationResult = authenticationContext.AcquireTokenAsync(aadResource, credential).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            // Create and get JWT token
            return authenticationResult.CreateAuthorizationHeader();
        }
    }
}
