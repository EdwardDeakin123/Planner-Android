using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DragAndDropDemo
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

        //TODO Cleanup parameters are POST or GET requests.

        public Backend()
        {
            _Parameters = new List<BackendParameter>();
        }

        //TODO Make sure everything is async at some stage to prevent the UI from locking.

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
            string queryString = GetQueryString();

            // Create a request, set the content type and method.
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.Method = "POST";

            // If the query string is not empty,
            if (queryString != "")
            {
                // Encode the query string. Found information here:
                // http://stackoverflow.com/questions/4015324/http-request-with-post
                var data = Encoding.ASCII.GetBytes(queryString);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                // Write the data to the stream.
                using(var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            // Submit the request.
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Not sure if I need to do anything with this response.
            }
        }
    }
}