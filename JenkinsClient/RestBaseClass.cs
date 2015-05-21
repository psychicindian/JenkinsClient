using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JenkinsClient
{
    public class RestApiService
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private int MaxRetryCount = 15;
        private int MaxRetrySleepInMs = 10000;
        private static string _userName = string.Empty;
        private static string _apiToken = string.Empty;
        public RestApiService(string userName, string apiToken)
        {
            _userName = userName;
            _apiToken = apiToken;
        }
        public HttpWebRequest CreateHttpWebRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] credentialBuffer = new UTF8Encoding().GetBytes(_userName + ":" + _apiToken);
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(credentialBuffer);
            request.PreAuthenticate = true;
            return request;
        }

        public HttpWebResponse GetWebResponse(string url, string postData = null)
        {
            HttpWebResponse response = null;
            RetryOnException<Exception>(() =>
            {
                var request = CreateHttpWebRequest(url);
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                if (!String.IsNullOrEmpty(postData))
                {
                    var data = Encoding.ASCII.GetBytes(postData);
                    // Write the data to the request stream.
                    dataStream.Write(data, 0, data.Length);
                    // Close the Stream object.
                    dataStream.Close();
                }
                response = (HttpWebResponse)request.GetResponse();
                return response != null;
            });
            response.Close();
            return response;
        }

        public T GetWebResponse<T>(string url, string postData = null)
        {
            T response = default(T);
            HttpWebResponse webResponse = null;
            RetryOnException<Exception>(() =>
            {
                var webRequest = CreateHttpWebRequest(url);
                Stream dataStream = webRequest.GetRequestStream();
                if (!String.IsNullOrEmpty(postData))
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                    webRequest.ContentLength = byteArray.Length;
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json;charset=utf-8";
                webResponse = (HttpWebResponse)webRequest.GetResponse();
                return webResponse != null;
            });
            Stream responseStream = webResponse.GetResponseStream();
            if (responseStream == null) return response;
            var reader = new StreamReader(responseStream);
            var responseFromServer = reader.ReadToEnd();
            webResponse.Close();
            responseStream.Close();
            //Log.Debug("Response From Server: " + responseFromServer);
            response = JsonConvert.DeserializeObject<T>(responseFromServer);
            return response;
        }

        //public static T Deserialize<T>(string stringToDeserialize)
        //{
        //    using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(stringToDeserialize)))
        //    {
        //        var ser = new DataContractJsonSerializer(typeof(T));
        //        var obj = (T)ser.ReadObject(memoryStream);
        //        return obj;
        //    }
        //}

        #region RetryHandler

        public void RetryOnException<T>(Func<bool> action) where T : Exception
        {
            //const int maxRetryCount = 5;
            //const int maxRetrySleepInMs = 10000;
            var currentRetryCount = 0;
            var sleepTimeInMs = GetSleepTimeIncrement(10000, 5, currentRetryCount);

            do
            {
                currentRetryCount++;
                try
                {
                    if (action())
                    {
                        Log.Debug("Action completed successfully. No more retries.");
                        break;
                    }
                }
                catch (T e)
                {
                    Log.Error(e);
                    Log.Error(string.Format("Exception of type {0} was caught.", e.GetType()));
                    if (currentRetryCount > MaxRetryCount)
                    {
                        Log.Error(
                            string.Format(
                                "Exception was caught while retrying.Exception of type {0} was caught while retying action. Exceeded max number of retries {1}.",
                                e.GetType(), MaxRetryCount));
                        throw;
                    }
                }

                if (currentRetryCount > MaxRetryCount)
                {
                    Log.Debug(string.Format("Exceeded max retry count: {0}", MaxRetryCount));
                    break;
                }

                if (sleepTimeInMs <= 0) continue;
                Log.Debug(string.Format("Retry count: {0}. Sleeping for {1} ms", currentRetryCount, sleepTimeInMs));
                Thread.Sleep(sleepTimeInMs);
                sleepTimeInMs = GetSleepTimeIncrement(MaxRetrySleepInMs, MaxRetryCount, currentRetryCount);
            } while (true);
        }

        private static int GetSleepTimeIncrement(int maxRetrySleepInMs, int maxRetryCount, int currentRetryCount)
        {
            //Can be modified to use a different function. Current function is arithmetic [Summation of i^2].
            if (maxRetryCount == 0)
            {
                return 0;
            }

            return (int)Math.Pow(currentRetryCount + 1, 2) * (6 * maxRetrySleepInMs) / (maxRetryCount * (maxRetryCount + 1) * (2 * maxRetryCount + 1));

        }

        #endregion
    }
}
