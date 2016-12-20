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
    class Backend<T> where T : IBackendType
    {
        // Variables to handle building the request.
        private string _Protocol = "http";
        // TODO need to be able to set this from the frontend.
        private string _Server = "192.168.0.13";
        private int _Port = 52029;

        //TODO Make sure everything is async at some stage to prevent the UI from locking.


        public async Task<T> Get()
        {
            // Function using GET to query the backend.
            // Build the URL from the global variables and query string.
            // TODO Add the query string here, maybe using a public getter / setter.
            // TODO Catch any exceptions that may be thrown here.
            string url = _Protocol + "://" + _Server + ":" + _Port + "/api/activity/get?activityid=1";

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

        public void Post()
        {

        }
    }
}