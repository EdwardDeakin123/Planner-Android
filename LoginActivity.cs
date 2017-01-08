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
using Front_End.Backend;

namespace Front_End
{
    [Activity(Label = "LoginActivity", Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            // Set the login layout.
            SetContentView(Resource.Layout.Login);

            //TODO Remove all of this unnecessary code.
            // Attempt to get the list of activities from the backend.
            //System.Diagnostics.Debug.WriteLine("Attempting to get an activity.");
            //BackendGet();

            // Attach the onclick listener to the login button.
            Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            btnLogin.Click += OnClick_btnLogin;
        }

        public async void OnClick_btnLogin(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button clicked!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            // Get the TextView's the contain the username and password.
            TextView usernameTxt = FindViewById<TextView>(Resource.Id.txtUsername);
            TextView passwordTxt = FindViewById<TextView>(Resource.Id.txtPassword);

            // Extract the text.
            string username = usernameTxt.Text;
            string password = passwordTxt.Text;

            if(username.Trim() == "")
            {
                // Username is empty, report an error here.
                return;
            }

            if(password.Trim() == "")
            {
                // Password is empty, report an error here.
                return;
            }

            // Use a try and catch to determine if the username or password is incorrect.
            try
            {
                // Create an instance of the backend.
                BackendUser backend = new BackendUser();
                await backend.Login(username, password);

                // If no exception is thrown, return to the previous activity.
                StartActivity(new Intent(this, typeof(MainActivity)));
            }
            catch(WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("StatusCode is " + ((HttpWebResponse)ex.Response).StatusCode);
                // Use WebException to get the status code returned from the API.
                if(((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    // User failed to authenticate. Tell them the username or password is incorrect.
                    System.Diagnostics.Debug.WriteLine("Incorrect username or password");
                }
                else
                {
                    // TODO maybe remove this.
                    // Rethrow the error if we don't match it, just to see what it is.
                    throw;
                }
            }
        }
    }
}