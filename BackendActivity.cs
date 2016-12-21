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
using System.Threading.Tasks;

namespace DragAndDropDemo
{
    // This is a helper class which manages API calls to the backend related to Activities.
    class BackendActivity : Backend
    {
        public BackendActivity() : base()
        {
            // Set the _Resource variable in the parent class. This defines which part of the API will be queried.
            _Resource = "Activity";
        }

        public async Task<List<ActivityModel>> GetAll()
        {
            // Set the command.
            _Command = "GetAll";

            return await GetRequestListAsync<ActivityModel>();
        }

        public async Task<ActivityModel> Get(int activityId)
        {
            // Set the command and create the query string.
            _Command = "Get";

            _Parameters.Add(new BackendParameter { Key = "activityId", Value = activityId.ToString() });

            return await GetRequestAsync<ActivityModel>();
        }
    }
}