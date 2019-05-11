using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Configuration;
using System.Net.Http.Headers;

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
        /// Property that gets the AuthorizationHeader
        /// </summary>
        public static string AuthorizationHeader
        {
            get
            {
                ServerSettings serverSettings = Settings.GetServerSettings();
                if (string.IsNullOrEmpty(authorizationHeader) || DateTime.UtcNow.AddSeconds(60) >= AuthenticationResult.ExpiresOn)
                {
                    UriBuilder uri = new UriBuilder(serverSettings.AzureAuthEndpoint)
                    {
                        Path = serverSettings.AADTenant
                    };

                    AuthenticationContext authenticationContext = new AuthenticationContext(uri.ToString());
                    var credential = new ClientCredential(serverSettings.WebAppId, serverSettings.WebAADKey);
                    AuthenticationResult = authenticationContext.AcquireTokenAsync(serverSettings.Ax7Endpoint, credential).Result;
                    authorizationHeader = AuthenticationResult.CreateAuthorizationHeader();
                }

                return authorizationHeader;
            }
        }

        /// <summary>
        /// Get a valid authentication header
        /// </summary>
        /// <returns>AuthenticationHeaderValue object</returns>
        public static AuthenticationHeaderValue GetValidAuthenticationHeader(bool _dropAuthHeader = false)
        {
            if(_dropAuthHeader == true)
            {
                AuthenticationHelper.DropAuthheader = _dropAuthHeader;
            }
            return AuthenticationHelper.ParseAuthenticationHeader(AuthenticationHelper.AuthorizationHeader);
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
