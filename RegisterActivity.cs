using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Front_End.Backend;
using System.Net;

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
            catch (TimeoutException)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);

                builder.SetMessage(GetString(Resource.String.unable_to_connect));
                builder.SetTitle(GetString(Resource.String.timeout));

                AlertDialog dialog = builder.Create();
                dialog.Create();
                dialog.Show();
            }
        }
        #endregion
    }
}