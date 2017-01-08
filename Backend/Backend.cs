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

namespace Front_End.Backend
{
    // This class is intended to manage all communication with the backend.
    // Information regarding how to query a REST API found here:
    // https://developer.xamarin.com/recipes/android/web_services/consuming_services/call_a_rest_web_service/
    class Backend
    {
        // Variables to handle building the request.
        private string _Protocol = "http";
        // TODO need to be able to set this from the frontend.
        private string _Server = "192.168.0.13";
        private int _Port = 52029;
        protected string _Resource = "";
        protected string _Command = "";
        protected List<BackendParameter> _Parameters;
        protected Dictionary<string, Cookie> _Cookies;
        protected CookieContainer _CookieContainer;

        //TODO Cleanup parameters are POST or GET requests.

        public Backend()
        {
            _Parameters = new List<BackendParameter>();
            _Cookies = new Dictionary<string, Cookie>();
            _CookieContainer = new CookieContainer();

            // Restore the saved cookie.
            // TODO: Add logic to check for expired cookies.
            LoadCookie();
        }

        private string GetUrlString()
        {
            // Build the URL from the global variables and query string.
            // TODO Catch any exceptions that may be thrown here.
            // TODO Perhaps rework the way the URL is built. Maybe use a string builder.
            // TODO Remove debug logging here.
            string url = _Protocol + "://" + _Server + ":" + _Port + "/api/" + _Resource + "/" + _Command;

            System.Diagnostics.Debug.WriteLine("The URL is : " + url);

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

            // Create a request, set the content type and method.
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Assign our own cookie container to the request object.
            request.CookieContainer = _CookieContainer;

            // Create a JsonSerializer to convert the stream from the server into an object.
            var serializer = new JsonSerializer();

            // Send the request asynchronously.
            using (WebResponse response = await request.GetResponseAsync())
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

            // Create a request, set the content type and method.
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";
            // Assign our own cookie container to the request object.
            request.CookieContainer = _CookieContainer;

            // Create a JsonSerializer to convert the stream from the server into an object.
            var serializer = new JsonSerializer();

            // Send the request asynchronously.
            using(WebResponse response = await request.GetResponseAsync())
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

            // Create a request, set the content type and method.
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            
            // Assign our own cookie container to the request object.
            request.CookieContainer = _CookieContainer;

            // Set a proxy - Used for debugging with Fiddler.
            // TODO Disable this later.
            //WebRequest.DefaultWebProxy = new WebProxy("192.168.0.13", 8888);

            request.Method = "POST";
            System.Diagnostics.Debug.WriteLine("Post Request JSON string!" + jsonString);
            request.ContentType = "application/json";

            // Write the JSON data to the stream.
            using(var stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(jsonString);
            }

            // Submit the request.
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                // Get the response. In the case of a Login request, this will be the
                // authentication cookie.
                if (response.Cookies.Count > 0)
                {
                    // Add the cookie to the current CookieContainer.
                    _CookieContainer.Add(response.Cookies);

                    // Save the cookie so it can be retrieved again later.
                    SaveCookie();
                }
            }
        }

        public async Task PutRequestAsync()
        {
            // Function using POST to write to the backend.
            // Get the URL.
            string url = GetUrlString();
            string jsonString = GetJSON();

            // Create a request, set the content type and method.
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));

            // Assign our own cookie container to the request object.
            request.CookieContainer = _CookieContainer;

            // Set a proxy - Used for debugging with Fiddler.
            // TODO Disable this later.
            //WebRequest.DefaultWebProxy = new WebProxy("192.168.0.13", 8888);

            request.Method = "PUT";
            System.Diagnostics.Debug.WriteLine("Post Request JSON string!" + jsonString);
            request.ContentType = "application/json";

            // Write the JSON data to the stream.
            using (var stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(jsonString);
            }

            // Submit the request.
            await request.GetResponseAsync();
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

            // Create a request, set the content type and method.
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "DELETE";

            // Assign our own cookie container to the request object.
            request.CookieContainer = _CookieContainer;

            // Send the request asynchronously.
            await request.GetResponseAsync();
        }


        private void LoadCookie()
        {
            // Access the SharedPreferences and retrieve the saved cookie.
            ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);

            string cookieName = sharedPreferences.GetString("cookieName", "");
            string cookieDomain = sharedPreferences.GetString("cookieDomain", "");
            string cookieValue = sharedPreferences.GetString("cookieValue", "");
            string cookiePath = sharedPreferences.GetString("cookiePath", "");

            if(cookieName != "")
            {
                // If the cookieName is not empty, assume the other values were correctly retrieved.
                // Create a cookie.
                Cookie newCookie = new Cookie(cookieName, cookieValue, cookiePath, cookieDomain);

                // Add the cookie to the collection.
                _CookieContainer.Add(newCookie);
            }
        }

        private void SaveCookie()
        {
            // Access the SharedPreferences and retrieve the saved cookie.
            ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
            ISharedPreferencesEditor spEditor = sharedPreferences.Edit();

            foreach(Cookie cookie in _CookieContainer.GetCookies(new Uri(GetUrlString())))
            {
                System.Diagnostics.Debug.WriteLine("The cookie is : " + cookie.ToString());
                
                // Save all of the cookie data into the shared preferences.
                spEditor.PutString("cookieName", cookie.Name);
                spEditor.PutString("cookieDomain", cookie.Domain);
                spEditor.PutString("cookieValue", cookie.Value);
                spEditor.PutString("cookiePath", cookie.Path);

                // Only want to get the first cookie, so only loop once.
                break;
            }

            // Apply the changes.
            spEditor.Apply();
        }
    }
}