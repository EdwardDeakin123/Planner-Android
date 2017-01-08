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
    [Activity(Label = "Login", Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            // Set the login layout.
            SetContentView(Resource.Layout.Login);

            // Attach the onclick listener to the login button.
            FindViewById<Button>(Resource.Id.btnLogin).Click += Login_OnClick;
            FindViewById<Button>(Resource.Id.btnCreateAccount).Click += Register_OnClick;
        }

        #region event handlers
        private async void Login_OnClick(object sender, EventArgs e)
        {
            // Get the TextView's the contain the username and password.
            TextView usernameTxt = FindViewById<TextView>(Resource.Id.txtUsername);
            TextView passwordTxt = FindViewById<TextView>(Resource.Id.txtPassword);

            // Extract the text.
            string username = usernameTxt.Text.Trim();
            string password = passwordTxt.Text.Trim();

            if(username == "")
            {
                // Username is empty, report an error here.
                return;
            }

            if(password == "")
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

        private void Register_OnClick(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(RegisterActivity)));
        }
        #endregion
    }
}