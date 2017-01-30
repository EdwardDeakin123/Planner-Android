using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Android.Webkit;
using Android.Content;
using Android.Preferences;
using Front_End.Exceptions;

namespace Front_End.Backend
{
    // This class is intended to manage all communication with the backend.
    // Information regarding how to query a REST API found here:
    // https://developer.xamarin.com/recipes/android/web_services/consuming_services/call_a_rest_web_service/
    class Backend
    {
        // Variables to handle building the request.
        private string _Protocol = "http";
        private string _Server;
        private int _Port;
        protected string _Resource = "";
        protected string _Command = "";
        protected List<BackendParameter> _Parameters;
        protected Dictionary<string, Cookie> _Cookies;
        protected CookieContainer _CookieContainer;
        protected Preferences _Preferences;

        // Define the timeout for the requests.
        protected const int TIMEOUT = 8000;

        public Backend()
        {
            _Parameters = new List<BackendParameter>();
            _Cookies = new Dictionary<string, Cookie>();
            _CookieContainer = new CookieContainer();
            _Preferences = new Preferences();

            // Get the server and port from the saved preferences.
            _Server = _Preferences.ServerAddress;
            _Port = _Preferences.ServerPort;

            // Restore the saved cookie, if it exists.
            Cookie savedCookie = _Preferences.AuthenticationCookie;

            // If the cookie exists, 
            if (savedCookie != default(Cookie))
                _CookieContainer.Add(savedCookie);
        }

        #region utility
        private string GetUrlString()
        {
            // Build the URL from the global variables and query string.
            string url = _Protocol + "://" + _Server + ":" + _Port + "/api/" + _Resource + "/" + _Command;
            return url;
        }

        private string GetQueryString()
        {
            string queryString = "";

            // Check that parameters is not empty.
            if(_Parameters.Count() > 0)
            {
                // Loop through the parameters and add them to the query string.
                foreach(BackendParameter param in _Parameters)
                {
                    // If this is not the first parameter, make sure to include the and symbol.
                    if(queryString != "")
                    {
                        queryString += "&";
                    }

                    // Use UrlEncode to make sure there are no invalid characters in the strings.
                    queryString += WebUtility.UrlEncode(param.Key) + "=" + WebUtility.UrlEncode(param.Value);
                }
            }

            return queryString;
        }

        private string GetJSON()
        {
            // Create a JSON string that can be passed to the backend.
            string jsonString = "";

            // Check that parameters is not empty.
            if (_Parameters.Count() > 0)
            {
                // Loop through the parameters and add them to the query string.
                foreach (BackendParameter param in _Parameters)
                {
                    // If this is not the first parameter, make sure to include the and symbol.
                    if (jsonString != "")
                    {
                        jsonString += ", ";
                    }

                    // Use UrlEncode to make sure there are no invalid characters in the strings.
                    jsonString += '"' + param.Key + '"' + ": " + '"' + param.Value + '"';
                }
            }

            jsonString = "{ " + jsonString + " }";

            return jsonString;
        }

        private Task<HttpWebResponse> TaskWithTimeout(Task<WebResponse> task, int duration)
        {
            // Implement a timeout feature to be used with GetResponseAsync which doesn't throw an exception on timeout.
            // Code modified from: http://stackoverflow.com/questions/13838088/how-to-define-a-more-aggressive-timeout-for-httpwebrequest
            return Task.Factory.StartNew(() =>
            {
                bool b = task.Wait(duration);
                if (b)
                {
                    return (HttpWebResponse)task.Result;
                }

                // Timeout reached, throw an exception.
                throw new BackendTimeoutException("Reached timeout while trying to communicate with the backend");
            });
        }

        private HttpWebRequest GetWebRequest(string url)
        {
            // Create a request, set the content type.
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.Timeout = TIMEOUT;

            //request.Proxy = new WebProxy("192.168.0.13", 8888);

            // Assign the cookie container to the request object, this authenticates this request.
            request.CookieContainer = _CookieContainer;
            request.ContentType = "application/json";

            return request;
        }
        #endregion

        #region requests
        public async Task<List<T>> GetRequestListAsync<T>() where T : IBackendType
        {
            // Using Generics so I can have a single function for getting elements of any type from the API.
            // Function using GET to query the backend. This variant is meant to get a List of objects
            // from the backend.
            // Get the URL.
            string url = GetUrlString();
            string queryString = GetQueryString();

            // if the query string is not empty, append it to the URL.
            if (queryString != "")
            {
                url += "?" + queryString;
            }

            // Create a request, and set the method
            HttpWebRequest request = GetWebRequest(url);
            request.Method = "GET";

            // Create a JsonSerializer to convert the stream from the server into an object.
            var serializer = new JsonSerializer();

            // Send the request asynchronously.
            using (WebResponse response = await TaskWithTimeout(request.GetResponseAsync(), TIMEOUT))
            {
                // Get a stream of the HTTP response.
                using (Stream stream = response.GetResponseStream())
                {
                    // Pipe the stream into a StreamReader, which can then be processed using the Newtonsoft Json library.
                    // http://stackoverflow.com/questions/26232569/newtonsoft-json-deserialize-using-httpwebresponse
                    StreamReader streamReader = new StreamReader(stream);

                    // Read the entire stream and convert it to a string, which can then be deserialized into an object.
                    string json = streamReader.ReadToEnd();

                    return JsonConvert.DeserializeObject<List<T>>(json);
                }
            }
        }

        public async Task<T> GetRequestAsync<T>() where T : IBackendType
        {
            // Function using GET to query the backend.
            // Get the URL.
            string url = GetUrlString();
            string queryString = GetQueryString();

            // if the query string is not empty, append it to the URL.
            if (queryString != "")
            {
                url += "?" + queryString;
            }

            // Create a request, and set the method
            HttpWebRequest request = GetWebRequest(url);
            request.Method = "GET";

            // Create a JsonSerializer to convert the stream from the server into an object.
            var serializer = new JsonSerializer();

            // Send the request asynchronously.
            using(WebResponse response = await TaskWithTimeout(request.GetResponseAsync(), TIMEOUT))
            {
                // Get a stream of the HTTP response.
                using(Stream stream = response.GetResponseStream())
                {
                    // Pipe the stream into a StreamReader, which can then be processed using the Newtonsoft Json library.
                    // http://stackoverflow.com/questions/26232569/newtonsoft-json-deserialize-using-httpwebresponse
                    StreamReader streamReader = new StreamReader(stream);

                    // Read the entire stream and convert it to a string, which can then be deserialized into an object.
                    string json = streamReader.ReadToEnd();

                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
        }

        public async Task PostRequestAsync()
        {
            // Function using POST to write to the backend.
            // Get the URL.
            string url = GetUrlString();
            string jsonString = GetJSON();

            // Create a request, and set the method
            HttpWebRequest request = GetWebRequest(url);
            request.Method = "POST";

            // Write the JSON data to the stream.
            using(var stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(jsonString);
            }

            // Submit the request.
            using (HttpWebResponse response = (HttpWebResponse)await TaskWithTimeout(request.GetResponseAsync(), TIMEOUT))
            {
                // Get the response. In the case of a Login request, this will be the
                // authentication cookie.
                if (response.Cookies.Count > 0)
                {
                    // Add the cookie to the current CookieContainer.
                    _CookieContainer.Add(response.Cookies);

                    // Save the cookie so it can be retrieved again later.
                    foreach (Cookie newCookie in response.Cookies)
                    {
                        // Save the cookie to Preferences and then leave the loop as we only want the first cookie.
                        _Preferences.AuthenticationCookie = newCookie;
                        break;
                    }
                }
            }
        }

        public async Task PutRequestAsync()
        {
            // Function using POST to write to the backend.
            // Get the URL.
            string url = GetUrlString();
            string jsonString = GetJSON();

            // Create a request, and set the method
            HttpWebRequest request = GetWebRequest(url);

            request.Method = "PUT";

            // Write the JSON data to the stream.
            using (var stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(jsonString);
            }

            // Submit the request.
            await TaskWithTimeout(request.GetResponseAsync(), TIMEOUT);
        }

        public async Task DeleteRequestAsync()
        {
            // Function using DELETE to query the backend.
            // Get the URL.
            string url = GetUrlString();
            string queryString = GetQueryString();

            // if the query string is not empty, append it to the URL.
            if (queryString != "")
            {
                url += "?" + queryString;
            }

            // Create a request, and set the method
            HttpWebRequest request = GetWebRequest(url);
            request.Method = "DELETE";

            // Send the request asynchronously.
            await TaskWithTimeout(request.GetResponseAsync(), TIMEOUT);
        }
        #endregion
    }
}