using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Front_End.Backend;
using System.Net;
using Front_End.Exceptions;
using Android.Views;

namespace Front_End
{
    [Activity(Label = "Register")]
    public class RegisterFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Return the layout to use in this fragment.
            return inflater.Inflate(Resource.Layout.Register, container, false);
        }


        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            // Attach to the Create Account onclick event
            View.FindViewById<Button>(Resource.Id.btnCreateAccount).Click += Register_OnClick;
        }

        #region event handlers
        private async void Register_OnClick(object sender, EventArgs e)
        {
            // Collect all of the data from the UI.
            string firstname = View.FindViewById<EditText>(Resource.Id.etFirstName).Text;
            string lastname = View.FindViewById<EditText>(Resource.Id.etLastName).Text;
            string username = View.FindViewById<EditText>(Resource.Id.etUsername).Text;
            string password = View.FindViewById<EditText>(Resource.Id.etPassword).Text;

            if (firstname == "" || lastname == "" || username == "" || password == "")
            {
                // At least one field is empty.
                View.FindViewById<TextView>(Resource.Id.tvErrors).Text = GetString(Resource.String.register_required_fields);
                return;
            }

            try
            {
                // Create an instance of the backend and register the user.
                BackendUser backend = new BackendUser();
                await backend.Register(firstname, lastname, username, password);

                // Switch back to the login activity
                Fragment fragment = new LoginFragment();

                // Load the fragment.
                FragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content, fragment)
                    .Commit();
            }
            catch (WebException ex)
            {
                // Use WebException to get the status code returned from the API.
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Forbidden)
                {
                    // One of the fields is empty.
                    View.FindViewById<TextView>(Resource.Id.tvErrors).Text = GetString(Resource.String.register_required_fields);
                }
                else if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Conflict)
                {
                    // Username is taken.
                    View.FindViewById<TextView>(Resource.Id.tvErrors).Text = GetString(Resource.String.register_username_taken);
                }
                else
                {
                    View.FindViewById<TextView>(Resource.Id.tvErrors).Text = GetString(Resource.String.unknown_error);
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
            AlertDialog.Builder builder = new AlertDialog.Builder(this.Activity);

            builder.SetMessage(message);
            builder.SetTitle(title);

            AlertDialog dialog = builder.Create();
            dialog.Create();
            dialog.Show();
        }
        #endregion
    }
}