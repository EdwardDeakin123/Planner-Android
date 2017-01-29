using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Front_End.Backend;
using Front_End.Models;
using System.Net;
using Front_End.Exceptions;
using Android.Views;
using System.Collections.Generic;
using System.Linq;

namespace Front_End
{
    [Activity(Label = "Edit Activity Log")]
    public class ActivityLogEditFragment : Fragment
    {
        private List<ActivityLogModel> _ActivityLogs;
        private int _ActivityLogIndex;
        private int _PlannerMode;
        private Preferences _Preferences;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create an instance of preferences, this is used to get configuration settings from shared preferences.
            _Preferences = new Preferences();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Get the activity log id from the bundle.
            int activityLogId = this.Arguments.GetInt("activityLogId", -1);
            // Get the type of planner that opened the edit fragment. This will be used to return to the correct type (daily or weekly).
            _PlannerMode = this.Arguments.GetInt("plannerMode", -1);

            if (activityLogId > -1)
            {
                // Get the Activity Logs from the local cache.
                GetActivityLogs();

                // Get the Index of the activity log that's being edited.
                _ActivityLogIndex = _ActivityLogs.FindIndex(actLog => actLog.ActivityLogId == activityLogId);
            }
            else
            {
                //TODO Report an error here
            }

            // Return the layout to use in this fragment.
            return inflater.Inflate(Resource.Layout.ActivityLogEdit, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // The view has been created, assign the event handlers.
            // Attach the click event on the save and delete buttons.
            View.FindViewById<Button>(Resource.Id.btnSave).Click += Save_OnClick;
            View.FindViewById<Button>(Resource.Id.btnDelete).Click += Delete_OnClick;

            // Update all of the UI with data from the activity log.
            UpdateUI();
        }

        #region backend
        private void UpdateUI()
        {
            // Update the interface elements with data from the backend.
            View.FindViewById<TextView>(Resource.Id.tvActivityName).Text = _ActivityLogs[_ActivityLogIndex].Activity.ActivityName;

            TimePicker startTime = View.FindViewById<TimePicker>(Resource.Id.tpStartTime);
            TimePicker endTime = View.FindViewById<TimePicker>(Resource.Id.tpEndTime);

            startTime.Hour = _ActivityLogs[_ActivityLogIndex].StartTime.Hour;
            startTime.Minute = _ActivityLogs[_ActivityLogIndex].StartTime.Minute;
            endTime.Hour = _ActivityLogs[_ActivityLogIndex].EndTime.Hour;
            endTime.Minute = _ActivityLogs[_ActivityLogIndex].EndTime.Minute;
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
                TimePicker startTime = View.FindViewById<TimePicker>(Resource.Id.tpStartTime);
                TimePicker endTime = View.FindViewById<TimePicker>(Resource.Id.tpEndTime);

                // Create timespan's that hold the updated time for this activity log.
                TimeSpan newStartTime = new TimeSpan(startTime.Hour, startTime.Minute, 0);
                TimeSpan newEndTime = new TimeSpan(endTime.Hour, endTime.Minute, 0);

                // Update the startStart and endTimes.
                DateTime modStart = _ActivityLogs[_ActivityLogIndex].StartTime.Date + newStartTime;
                DateTime modEnd = _ActivityLogs[_ActivityLogIndex].EndTime.Date + newEndTime;

                // Check if Demo mode is enabled, if not - update the acitivity log in the backend.
                // Otherwise, update the activity log and send it back to the Planner activity.
                if (!_Preferences.DemoMode)
                {
                    // Submit these changes to the backend.
                    await backend.Update(_ActivityLogs[_ActivityLogIndex].ActivityLogId, modStart, modEnd);
                }
                else
                {
                    // Update the activity log.
                    _ActivityLogs[_ActivityLogIndex].StartTime = modStart;
                    _ActivityLogs[_ActivityLogIndex].EndTime = modEnd;

                    // Save the activity logs to disk so they can be view on the planner.
                    CacheActivityLogs();
                }

                // Return to the previous fragment.
                FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
                Fragment fragment;

                // Create an instance of the planner fragment based on the senders view type (weekly or daily).
                if(_PlannerMode == PlannerFragment.DAILY_VIEW)
                    fragment = new PlannerDailyFragment();
                else
                    fragment = new PlannerWeeklyFragment();

                // Create a bundle that can be passed back to the planner.
                Bundle fragmentBundle = new Bundle();

                // Send the activity log's date back to the planner, so that the updated log is in the view.
                fragmentBundle.PutInt("viewDateYear", _ActivityLogs[_ActivityLogIndex].StartTime.Year);
                fragmentBundle.PutInt("viewDateMonth", _ActivityLogs[_ActivityLogIndex].StartTime.Month);
                fragmentBundle.PutInt("viewDateDay", _ActivityLogs[_ActivityLogIndex].StartTime.Day);

                // Attach the bundle to the fragment and commit it.
                fragment.Arguments = fragmentBundle;
                fragmentTransaction.Replace(Resource.Id.content, fragment).Commit();
            }
            catch (System.Net.WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    // The user is not logged in. Move to the Login activity.
                    StartActivity(new Intent(this.Activity, typeof(LoginFragment)));
                }
                System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
            }
        }

        private async void Delete_OnClick(object sender, EventArgs e)
        {
            // Prepare the Fragment manager.
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            Fragment fragment;

            // Return to the planner fragment.
            // Create an instance of the planner fragment based on the senders view type (weekly or daily).
            if (_PlannerMode == PlannerFragment.DAILY_VIEW)
                fragment = new PlannerDailyFragment();
            else
                fragment = new PlannerWeeklyFragment();

            // Create a bundle that can be passed back to the planner.
            Bundle fragmentBundle = new Bundle();

            // Send the activity log's date back to the planner.
            fragmentBundle.PutInt("viewDateYear", _ActivityLogs[_ActivityLogIndex].StartTime.Year);
            fragmentBundle.PutInt("viewDateMonth", _ActivityLogs[_ActivityLogIndex].StartTime.Month);
            fragmentBundle.PutInt("viewDateDay", _ActivityLogs[_ActivityLogIndex].StartTime.Day);

            // Attach the fragment bundle.
            fragment.Arguments = fragmentBundle;

            if (!_Preferences.DemoMode)
            {
                // Demo mode is disabled. Update the activity log in the backend.
                try
                {
                    // Prepare the ActivityLog backend object.
                    BackendActivityLog backend = new BackendActivityLog();

                    // Submit these changes to the backend.
                    await backend.Delete(_ActivityLogs[_ActivityLogIndex].ActivityLogId);

                    // Switch to the fragment.
                    fragmentTransaction.Replace(Resource.Id.content, fragment).Commit();
                }
                catch (System.Net.WebException ex)
                {
                    if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // The user is not logged in. Move to the Login activity.
                        StartActivity(new Intent(this.Activity, typeof(LoginFragment)));
                    }
                    System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
                }
            }
            else
            {
                // Demo mode is enabled, delete the log from the list and write it to disk.
                _ActivityLogs.RemoveAt(_ActivityLogIndex);
                CacheActivityLogs();

                // Switch to the fragment.
                fragmentTransaction.Replace(Resource.Id.content, fragment).Commit();
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

        private void GetActivityLogs()
        {
            // Retrieve the list of activity logs from the cache.
            try
            {
                // Get the serialized data from the disk.
                string activityXml = Utility.ReadFromFile("activitylogs.txt");
                ObjectCache<List<ActivityLogModel>> activityLogCache = Utility.DeserializeFromString<ObjectCache<List<ActivityLogModel>>>(activityXml);

                // Check if the cached object has expired, if it has - create a new object.
                // Otherwise return the cached object.
                if (activityLogCache.Expired)
                    _ActivityLogs = new List<ActivityLogModel>();
                else
                    _ActivityLogs = activityLogCache.Object;
            }
            catch
            {
                // There is no cache or an error occurred. Do nothing.
                _ActivityLogs = new List<ActivityLogModel>();
            }
        }

        private void CacheActivityLogs()
        {
            // Add the activity logs and activities to an ObjectCacheModel object. This will help manage stale items in the cache.
            ObjectCache<List<ActivityLogModel>> activityLogCache = new ObjectCache<List<ActivityLogModel>>() { Object = _ActivityLogs };

            // Serialize these objects and write them to the disk.
            Utility.WriteToFile(Utility.SerializeToString(activityLogCache), "activitylogs.txt");
        }
        #endregion
    }
}