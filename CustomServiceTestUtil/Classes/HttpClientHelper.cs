using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CustomServiceTestUtil
{
    /// <summary>
    /// Helper class for Enqueue/ Job status http requests
    /// </summary>
    class HttpClientHelper
    {

        /// <summary>
        /// Post request
        /// </summary>
        /// <param name="uri">Enqueue endpoint URI</param>
        /// <param name="authenticationHeader">Authentication header</param>
        /// <param name="bodyStream">Body stream</param>        
        /// <param name="message">ActivityMessage context</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendPostRequestAsync(Uri uri, Stream bodyStream, bool _dropAuthHeader = false, string externalCorrelationHeaderValue = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            using (HttpClientHandler handler = new HttpClientHandler() { UseCookies = false })
            {
                using (HttpClient httpClient = new HttpClient(handler))
                {
                    httpClient.DefaultRequestHeaders.Authorization = AuthenticationHelper.GetValidAuthenticationHeader(_dropAuthHeader);

                    // Add external correlation id header id specified and valid
                    if (!string.IsNullOrEmpty(externalCorrelationHeaderValue))
                    {
                        httpClient.DefaultRequestHeaders.Add(App.ExternalCorrelationHeader, externalCorrelationHeaderValue);
                    }

                    if (bodyStream != null)
                    {
                        using (StreamContent content = new StreamContent(bodyStream))
                        {
                            return await httpClient.PostAsync(uri, content);
                        }
                    }
                }
            }

            return new HttpResponseMessage()
            {
                Content = new StringContent("Request failed at client.", Encoding.ASCII),
                StatusCode = System.Net.HttpStatusCode.PreconditionFailed
            };
        }
        public async Task<HttpResponseMessage> SendPostRequestAsync(Uri uri, string data, string externalCorrelationHeaderValue = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            using (HttpClientHandler handler = new HttpClientHandler() { UseCookies = false })
            {
                using (HttpClient httpClient = new HttpClient(handler))
                {
                    httpClient.DefaultRequestHeaders.Authorization = AuthenticationHelper.GetValidAuthenticationHeader();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

                    // Add external correlation id header id specified and valid
                    if (!string.IsNullOrEmpty(externalCorrelationHeaderValue))
                    {
                        httpClient.DefaultRequestHeaders.Add(App.ExternalCorrelationHeader, externalCorrelationHeaderValue);
                    }

                    if (!string.IsNullOrEmpty(data))
                    {
                        return await httpClient.PostAsync(uri, new StringContent(data, Encoding.UTF8, "application/json"));
                    }
                }
            }

            return new HttpResponseMessage()
            {
                Content = new StringContent("Request failed at client.", Encoding.ASCII),
                StatusCode = System.Net.HttpStatusCode.PreconditionFailed
            };
        }
        /// <summary>
        /// Http Get requests for use with JobStatus API
        /// </summary>
        /// <param name="uri">Request URI</param>
        /// <returns>Task of type HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> GetRequestAsync(Uri uri)
        {
            HttpResponseMessage responseMessage;

            using (HttpClientHandler handler = new HttpClientHandler() { UseCookies = false })
            {
                using (HttpClient httpClient = new HttpClient(handler))
                {
                    httpClient.DefaultRequestHeaders.Authorization = AuthenticationHelper.GetValidAuthenticationHeader();
                    responseMessage = await httpClient.GetAsync(uri).ConfigureAwait(false);
                }
            }
            return responseMessage;
        }
    }
}