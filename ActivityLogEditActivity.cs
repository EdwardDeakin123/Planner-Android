using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Front_End.Backend;
using Front_End.Models;
using System.Net;
using Front_End.Exceptions;

namespace Front_End
{
    [Activity(Label = "Edit Activity Log")]
    public class ActivityLogEditActivity : Activity
    {
        private ActivityLogModel _ActivityLog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ActivityLogEdit);

            // Get the activity log id from the intent.
            int activityLogId = Intent.GetIntExtra("activityLogId", -1);

            // Retrieve the Activity Log from the Backend.
            GetActivityLog(activityLogId);

            // Attach the click event on the save and delete buttons.
            FindViewById<Button>(Resource.Id.btnSave).Click += Save_OnClick;
            FindViewById<Button>(Resource.Id.btnDelete).Click += Delete_OnClick;
        }

        #region backend
        private async void GetActivityLog(int activityLogId)
        {
            try
            {
                // Get the ActivityLog from the backend.
                BackendActivityLog backend = new BackendActivityLog();
                _ActivityLog = await backend.Get(activityLogId);

                // Update the interface elements with data from the backend.
                FindViewById<TextView>(Resource.Id.tvActivityName).Text = _ActivityLog.Activity.ActivityName;

                TimePicker startTime = FindViewById<TimePicker>(Resource.Id.tpStartTime);
                TimePicker endTime = FindViewById<TimePicker>(Resource.Id.tpEndTime);


                startTime.Hour = _ActivityLog.StartTime.Hour;
                startTime.Minute = _ActivityLog.StartTime.Minute;
                endTime.Hour = _ActivityLog.EndTime.Hour;
                endTime.Minute = _ActivityLog.EndTime.Minute;
            }
            catch (System.Net.WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    // The user is not logged in. Move to the Login activity.
                    StartActivity(new Intent(this, typeof(LoginActivity)));
                }
                System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
            }
            catch (BackendTimeoutException)
            {
                // Display a popup.
                DisplayAlert(GetString(Resource.String.timeout), GetString(Resource.String.timeout_message));
            }
            catch (AggregateException e)
            {
                // Catch the aggregate exception, this might be thrown by the asynchronous tasks in the backend.
                // Handle any of the types that we are aware of.
                // Managing of aggregate exceptions modified from code found here: https://msdn.microsoft.com/en-us/library/dd537614%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396

                e.Handle((x) =>
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

        #region event handlers
        private async void Save_OnClick(object sender, EventArgs e)
        {
            try
            {
                // Prepare the ActivityLog backend object.
                BackendActivityLog backend = new BackendActivityLog();

                // Get data from the UI
                TimePicker startTime = FindViewById<TimePicker>(Resource.Id.tpStartTime);
                TimePicker endTime = FindViewById<TimePicker>(Resource.Id.tpEndTime);

                // Create timespan's that hold the updated time for this activity log.
                TimeSpan newStartTime = new TimeSpan(startTime.Hour, startTime.Minute, 0);
                TimeSpan newEndTime = new TimeSpan(endTime.Hour, endTime.Minute, 0);

                // Update the startStart and endTimes.
                DateTime modStart = _ActivityLog.StartTime.Date + newStartTime;
                DateTime modEnd = _ActivityLog.EndTime.Date + newEndTime;

                // Submit these changes to the backend.
                await backend.Update(_ActivityLog.ActivityLogId, modStart, modEnd);

                // Return to the Main activity after updating.
                Finish();
            }
            catch (System.Net.WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    // The user is not logged in. Move to the Login activity.
                    StartActivity(new Intent(this, typeof(LoginActivity)));
                }
                System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
            }
        }

        private async void Delete_OnClick(object sender, EventArgs e)
        {
            try
            {
                // Prepare the ActivityLog backend object.
                BackendActivityLog backend = new BackendActivityLog();

                // Submit these changes to the backend.
                await backend.Delete(_ActivityLog.ActivityLogId);

                // Return to the Main activity after updating.
                Finish();
            }
            catch (System.Net.WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    // The user is not logged in. Move to the Login activity.
                    StartActivity(new Intent(this, typeof(LoginActivity)));
                }
                System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
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