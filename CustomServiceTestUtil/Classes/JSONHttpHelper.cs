using AuthenticationUtility;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CustomServiceTestUtil
{
    class JSONHttpHelper
    {
        private Page page;
        public DateTime begin { get; set; }
        public string serviceMethod { get; set; }
        public string json { get; set; }
        public JSONHttpHelper() 
        {
         
        }
        public async void callServiceSync(string _json,  DateTime _begin, string _serviceMethod)
        {
            begin = _begin;
            serviceMethod = _serviceMethod;

            try
            {
                ServerSettings serverSettings = Settings.GetServerSettings();
                UriBuilder serviceUri = new UriBuilder(serverSettings.Ax7Endpoint)
                {
                    Path = string.Format("{0}{1}", App.CustomServices, _serviceMethod)
                };

                var request = HttpWebRequest.Create(serviceUri.Uri);
                request.Headers[OAuthHelper.OAuthHeader] = await OAuthHelper.GetAuthenticationHeader();
                request.Method = "POST";

                using (var stream = request.GetRequestStream())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(_json);
                    }
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            string responseString = streamReader.ReadToEnd();                            
                            //page.logText("Http status code: " + System.Net.HttpStatusCode.OK.ToString());
                            //page.logText("Response: " + responseString);
                            ////var returnedContractList = returnedComplex.MeasurementOrder.MeasureTestLines;
                            //MeasurementOrder returnedComplex = JsonConvert.DeserializeObject<MeasurementOrder>(responseString);

                        }
                    }
                }


            }
            catch (SocketException ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(ex.ToString() + ex.ErrorCode.ToString(), EventLogEntryType.Error, 100);
                    //page.logText(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error, 100);
                    //page.logText(ex.ToString());
                }
            }


        }

        public void callService(string _json,Page _page, DateTime _begin,string _serviceMethod,bool _dropAuthHeader)
        {
            page = _page;
            begin = _begin;
            serviceMethod = _serviceMethod;
            Task runTask = Task.Factory.StartNew(() => callPostJSON(_json, _dropAuthHeader));
            Task.WaitAll(runTask);
        }
        private async Task<HttpResponseMessage> callPostJSON(string _json,bool _dropAuthHeader)
        {
            HttpResponseMessage response = null;
            try
            {
                ServerSettings serverSettings = Settings.GetServerSettings();
                UriBuilder serviceUri = new UriBuilder(serverSettings.Ax7Endpoint)
                {
                    Path = string.Format("{0}{1}", App.CustomServices, serviceMethod)
                };

                var httpClientHelper = new HttpClientHelper();
                Stream json = new MemoryStream(Encoding.UTF8.GetBytes(_json));
                response = await httpClientHelper.SendPostRequestAsync(serviceUri.Uri, json, _dropAuthHeader);

                DateTime end = DateTime.Now;

                if (page != null)
                {

                    if (page is IOutputMessages<string> caller)
                    {
                        caller.SetOutput(string.Format(Properties.Resources.CallTiming, (end - begin).TotalSeconds));
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            caller.SetOutput(Properties.Resources.Request + response.RequestMessage.ToString());
                            caller.SetOutput(Properties.Resources.Response + response.ToString());
                            caller.SetResponse(response.ToString());
                        }
                        else
                        {
                            Stream stream = await response.Content.ReadAsStreamAsync();
                            StreamReader reader = new StreamReader(stream);
                            string outtext = reader.ReadToEnd();
                            caller.SetResponse(outtext);
                            caller.SetOutput(Properties.Resources.HTTPStatusCode + System.Net.HttpStatusCode.OK.ToString());
                        }
                    }


                }
            }
            catch (SocketException ex)
            {
                using (EventLog eventLog = new EventLog(Properties.Resources.Application))
                {
                    eventLog.Source = Properties.Resources.Application;
                    eventLog.WriteEntry(ex.ToString() + ex.ErrorCode.ToString(), EventLogEntryType.Error, 100);
                    if (page is IOutputMessages<string> caller)
                    {
                        caller.SetOutput(Properties.Resources.ProcessingFailure + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog(Properties.Resources.Application))
                {
                    eventLog.Source = Properties.Resources.Application;
                    eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error, 100);
                }
                if (page is IOutputMessages<string> caller)
                {
                    caller.SetOutput(Properties.Resources.ProcessingFailure + ex.Message);
                }
            }
            return response;
        }
        private async void postJSONStream(string _json)
        {
            try
            {
                ServerSettings serverSettings = Settings.GetServerSettings();
                string servicePath = string.Format("{0}{1}{2}", serverSettings.Ax7Endpoint, App.CustomServices, serviceMethod);
                var request = HttpWebRequest.Create(servicePath);
                request.Headers[OAuthHelper.OAuthHeader] = await OAuthHelper.GetAuthenticationHeader();
                request.Method = "POST";

                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream());
                requestWriter.Write(_json);
                requestWriter.Close();

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            string responseString = streamReader.ReadToEnd();
                            Console.WriteLine(responseString);
                        }
                    }
                }

            }
            catch (SocketException ex)
            {
                using (EventLog eventLog = new EventLog(Properties.Resources.Application))
                {
                    eventLog.Source = Properties.Resources.Application;
                    eventLog.WriteEntry(ex.ToString() + ex.ErrorCode.ToString(), EventLogEntryType.Error, 100);
                    if (page is IOutputMessages<string> caller)
                    {
                        caller.SetOutput(Properties.Resources.ProcessingFailure + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog(Properties.Resources.Application))
                {
                    eventLog.Source = Properties.Resources.Application;
                    eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error, 100);
                }
                if (page is IOutputMessages<string> caller)
                {
                    caller.SetOutput(Properties.Resources.ProcessingFailure + ex.Message);
                }
            }

        }
    }
}