using Microsoft.Identity.Client;
using System;
using System.Configuration;
using System.Net.Http.Headers;
using System.Threading.Tasks;
namespace CustomServiceTestUtil
{
    /// <summary>
    /// Helper class that enables generating the
    /// auth header for Enqueue and JobStatus requests
    /// </summary>
    class AuthenticationHelper
    {
        public static string authorizationHeader;
        public static AuthenticationResult AuthenticationResult { get; private set; }
        public static bool DropAuthheader
        {
            set
            {
                if(value == true)
                {
                    authorizationHeader = string.Empty;
                }
            }
        }

        /// <summary>
        /// Method that gets the AuthorizationHeader
        /// </summary>
        public static async Task<string> GetAuthorizationHeader()
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
            authorizationHeader = result.CreateAuthorizationHeader();
            return authorizationHeader;
        }

        /// <summary>
        /// Get a valid authentication header
        /// </summary>
        /// <returns>AuthenticationHeaderValue object</returns>
        public static AuthenticationHeaderValue GetValidAuthenticationHeader(bool _dropAuthHeader = false)
        {
            if (_dropAuthHeader == true)
            {
                AuthenticationHelper.DropAuthheader = _dropAuthHeader;
            }
            Task<string> autHeader = GetAuthorizationHeader();
            string header = autHeader.Result;
            return AuthenticationHelper.ParseAuthenticationHeader(header);
        }

        static AuthenticationHeaderValue ParseAuthenticationHeader(string authorizationHeader)
        {
            string[] split = authorizationHeader.Split(' ');
            string scheme = split[0];
            string parameter = split[1];
            return new AuthenticationHeaderValue(scheme, parameter);
        }
    }
}
