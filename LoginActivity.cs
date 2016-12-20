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

namespace DragAndDropDemo
{
    [Activity(Label = "LoginActivity", MainLauncher = true, Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            // Set the login layout.
            SetContentView(Resource.Layout.Login);

            // Attempt to get the list of activities from the backend.
            System.Diagnostics.Debug.WriteLine("Attempting to get an activity.");
            BackendGet();
        }

        private async void BackendGet()
        {
            // Test getting activitys from the backend.
            Backend<ActivityModel> backend = new Backend<ActivityModel>();
            ActivityModel activity = await backend.Get();

            System.Diagnostics.Debug.WriteLine("Maybe got an activity...");
            System.Diagnostics.Debug.WriteLine("This one is " + activity.ActivityName);
        }
    }
}