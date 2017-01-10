using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net;
using Front_End.Backend;
using Front_End.Exceptions;

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

            if(username == "" || password == "")
            {
                FindViewById<TextView>(Resource.Id.tvErrors).Text = GetString(Resource.String.login_required_fields);
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
                    FindViewById<TextView>(Resource.Id.tvErrors).Text = GetString(Resource.String.login_incorrect);
                }
                else
                {
                    // Some other error was thrown.
                    FindViewById<TextView>(Resource.Id.tvErrors).Text = GetString(Resource.String.unknown_error);
                }
            }
            catch (BackendTimeoutException)
            {
                // Display a popup.
                DisplayAlert(GetString(Resource.String.timeout), GetString(Resource.String.timeout_message));
            }
            catch (AggregateException ex)
            {
                // Catch the aggregate exception, this might be thrown by the asynchronous tasks in the backend.
                // Handle any of the types that we are aware of.
                // Managing of aggregate exceptions modified from code found here: https://msdn.microsoft.com/en-us/library/dd537614%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396

                ex.Handle((x) =>
                {
                    if (x is WebException)
                    {
                        // Check for an inner exception of Socket Exception
                        if (x.InnerException is System.Net.Sockets.SocketException)
                        {
                            // There is an issue connecting to the backend.
                            DisplayAlert(GetString(Resource.String.connection_failed), GetString(Resource.String.connection_failed_message));
                        }

                        // This exception matched, return true.
                        return true;
                    }

                    // Was not able to handle the exception.
                    return false;
                });
            }
        }

        private void Register_OnClick(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(RegisterActivity)));
        }
        #endregion

        #region utility
        private void DisplayAlert(string title, string message)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetMessage(message);
            builder.SetTitle(title);

            AlertDialog dialog = builder.Create();
            dialog.Create();
            dialog.Show();
        }
        #endregion
    }
}