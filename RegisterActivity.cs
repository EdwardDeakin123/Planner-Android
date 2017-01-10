using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Front_End.Backend;
using System.Net;
using Front_End.Exceptions;

namespace Front_End
{
    [Activity(Label = "Register")]
    public class RegisterActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set the Register layout.
            SetContentView(Resource.Layout.Register);

            // Attach to the Create Account onclick event
            FindViewById<Button>(Resource.Id.btnCreateAccount).Click += Register_OnClick;
        }

        #region event handlers
        private async void Register_OnClick(object sender, EventArgs e)
        {
            // Collect all of the data from the UI.
            string firstname = FindViewById<EditText>(Resource.Id.etFirstName).Text;
            string lastname = FindViewById<EditText>(Resource.Id.etLastName).Text;
            string username = FindViewById<EditText>(Resource.Id.etUsername).Text;
            string password = FindViewById<EditText>(Resource.Id.etPassword).Text;

            if (firstname == "" || lastname == "" || username == "" || password == "")
            {
                // At least one field is empty.
                FindViewById<TextView>(Resource.Id.tvErrors).Text = GetString(Resource.String.register_required_fields);
                return;
            }

            try
            {
                // Create an instance of the backend and register the user.
                BackendUser backend = new BackendUser();
                await backend.Register(firstname, lastname, username, password);

                // If no exception is thrown, return to the previous activity.
                Finish();
            }
            catch (WebException ex)
            {
                // Use WebException to get the status code returned from the API.
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Forbidden)
                {
                    // One of the fields is empty.
                    FindViewById<TextView>(Resource.Id.tvErrors).Text = GetString(Resource.String.register_required_fields);
                }
                else if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Conflict)
                {
                    // Username is taken.
                    FindViewById<TextView>(Resource.Id.tvErrors).Text = GetString(Resource.String.register_username_taken);
                }
                else
                {
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