﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Front_End.Models;
using Front_End.Backend;
using Front_End.Exceptions;
using SQLite;
using Front_End.Database;

namespace Front_End
{
    [Activity(Label = "Activity Tracker", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
        // Layout elements.
        RelativeLayout _RLHover;
        RelativeLayout _RLDropzone;

        // The date of the current view.
        DateTime _ViewDate;

        // Global variables used with the demo mode (_FakeData).
        List<ActivityModel> _Activities;
        List<ActivityLogModel> _ActivityLogs;
        int _ActivityLogId;

        // List that contains the list of colors associated with each activity ID.
        List<KeyValuePair<int, Color>> _ActivityColors;

        // Set the DP taken up by one hour.
        private const int HOUR_DP = 60;

        // This variable is used to skip the backend and just use test data.
        bool _FakeData = false;

		protected override void OnCreate(Bundle bundle)
		{
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

            base.OnCreate(bundle);
	    
            // Using a global variable so we can simulate adding data when using the FakeData switch.
            _Activities = new List<ActivityModel>();
            _ActivityLogs = new List<ActivityLogModel>();
            _ActivityLogId = 0;
            _ActivityColors = new List<KeyValuePair<int, Color>>();

            // Assign the next and previous onclick events.
            FindViewById<ImageButton>(Resource.Id.ibNextDate).Click += NextDay_OnClick;
            FindViewById<ImageButton>(Resource.Id.ibPrevDate).Click += PreviousDay_OnClick;

            if(_ViewDate == default(DateTime))
            {
                DateTime currentTime = DateTime.Now;

                // If the current view date is not set, set it to the current day.
                _ViewDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);
            }

            // Get the dropzone and attach a drag event listener.
            _RLDropzone = FindViewById<RelativeLayout>(Resource.Id.rlDropzone);
            _RLDropzone.Drag += DropZone_Drag;
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Reload the planner when the activity is resumed.
            ReloadUI();

            TimesDatabase timeDb = new TimesDatabase();

            if (timeDb.FindLatest() != DateTime.Now.Day)
            {
                new Notification();
            }
        }
	
        #region backend
        private async void GetActivities()
        {
            try
            {
                // Store activities in a global variable so they can be referenced later without requiring another
                // connection to the backend.
                // Get activities from the backend.
                if (!_FakeData)
                {
                    // Get real data.
                    BackendActivity backend = new BackendActivity();
                    _Activities = await backend.GetAll();

                    System.Diagnostics.Debug.WriteLine("Got " + _Activities.Count + " activities");
                }
                else
                {
                    // Populate the page with fake data.
                    _Activities = new List<ActivityModel>()
                    {
                        new ActivityModel() { ActivityId = 1, ActivityName = "Fake Running" },
                        new ActivityModel() { ActivityId = 2, ActivityName = "Fake Sleeping" },
                        new ActivityModel() { ActivityId = 3, ActivityName = "Fake Jogging" },
                        new ActivityModel() { ActivityId = 4, ActivityName = "Fake Walking" }
                    };
                }

                System.Diagnostics.Debug.WriteLine("Got some activities.");

                foreach (ActivityModel act in _Activities)
                {
                    System.Diagnostics.Debug.WriteLine("Got activity: " + act.ActivityName);

                    Button actButton = new Button(this);
                    actButton.Text = act.ActivityName;

                    // Add the ID of this activity to the button's tag so it can later be identified.
                    actButton.SetTag(Resource.Id.activity_id, act.ActivityId);
                    actButton.LongClick += Activity_LongClick;

                    FindViewById<LinearLayout>(Resource.Id.llActivities).AddView(actButton);
                }
            }
            catch (WebException ex)
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

                            // The exception has been handled. Return true.
                            return true;
                        }
                        else
                        {
                            WebException wEx = (WebException)x;

                            if (((HttpWebResponse)wEx.Response).StatusCode == HttpStatusCode.Unauthorized)
                            {
                                // The user is not logged in. Move to the Login activity.
                                StartActivity(new Intent(this, typeof(LoginActivity)));

                                // The exception has been handled. Return true.
                                return true;
                            }

                            System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
                        }

                        System.Diagnostics.Debug.WriteLine("Got an aggregate exception...");
                    }

                    // Was not able to handle the exception.
                    return false;
                });
            }
        }

        private async void GetActivityLogs()
        {
            try
            {
                // Get activities from the backend.
                if (!_FakeData)
                {
                    // Get real data.
                    BackendActivityLog backend = new BackendActivityLog();
                    _ActivityLogs = await backend.GetByDate(_ViewDate);
                }

                System.Diagnostics.Debug.WriteLine("Got " + _ActivityLogs.Count + " activity logs");

                foreach (ActivityLogModel actLog in _ActivityLogs)
                {
                    Console.WriteLine("ActivityLog: " + actLog.Activity.ActivityName + " " + actLog.StartTime.ToString() + " " + actLog.EndTime.ToString());

                    // Display the Activity Log on the calendar.
                    AddActivityLogToCalendar(actLog);
                }
            }
            catch (WebException ex)
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

                            // The exception has been handled. Return true.
                            return true;
                        }
                        else
                        {
                            WebException wEx = (WebException)x;

                            if (((HttpWebResponse)wEx.Response).StatusCode == HttpStatusCode.Unauthorized)
                            {
                                // The user is not logged in. Move to the Login activity.
                                StartActivity(new Intent(this, typeof(LoginActivity)));

                                // The exception has been handled. Return true.
                                return true;
                            }

                            System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
                        }

                        System.Diagnostics.Debug.WriteLine("Got an aggregate exception...");
                    }

                    // Was not able to handle the exception.
                    return false;
                });
            }
        }

        private async void AddActivityLogToBackend(int activityId, DateTime startTime, DateTime endTime)
        {
            try
            {
                if (!_FakeData)
                {
                    // Get real data.
                    BackendActivityLog backend = new BackendActivityLog();
                    await backend.Add(activityId, startTime, endTime);
                }
                else
                {
                    // Create an ActivityLog and add it to the global list but don't push it to the database.
                    // Get the activity associated with this log.
                    ActivityModel activity = _Activities.Where(act => act.ActivityId == activityId).FirstOrDefault();
                    _ActivityLogs.Add(new ActivityLogModel() { ActivityLogId = _ActivityLogId, Activity = activity, StartTime = startTime, EndTime = endTime });

                    _ActivityLogId++;
                }
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

                            // The exception has been handled. Return true.
                            return true;
                        }
                        else
                        {
                            WebException wEx = (WebException)x;

                            if (((HttpWebResponse)wEx.Response).StatusCode == HttpStatusCode.Unauthorized)
                            {
                                // The user is not logged in. Move to the Login activity.
                                StartActivity(new Intent(this, typeof(LoginActivity)));

                                // The exception has been handled. Return true.
                                return true;
                            }

                            System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
                        }

                        System.Diagnostics.Debug.WriteLine("Got an aggregate exception...");
                    }

                    // Was not able to handle the exception.
                    return false;
                });
            }
        }
        #endregion

        #region event handlers
        private void Activity_LongClick(object sender, View.LongClickEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Activity Long Click... Starting Drag...");

            // Get the Activity button from the sender object.
            Button activityButton = (Button)sender;

            // Create clip data that will be attached to the drag operation.
            // Get the activityId from the Tag and assign it to the clip data.
            var data = ClipData.NewPlainText("activityId", activityButton.GetTag(Resource.Id.activity_id).ToString());

            // Start dragging and pass data
            // StartDrag was deprecated in API 24, check which version of Android we're running on and use the appropriate method.
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                activityButton.StartDragAndDrop(data, new View.DragShadowBuilder(activityButton), null, 0);
            }
            else
            {
                activityButton.StartDrag(data, new View.DragShadowBuilder(activityButton), null, 0);
            }
        }

        private void DropZone_Drag(object sender, View.DragEventArgs e)
        {
            // Get the event from the sender object.
            var dragEvent = e.Event;

            // Perform an action.
            switch (dragEvent.Action)
            {
                case DragAction.Ended:
                case DragAction.Started:
                    e.Handled = true;
                    break;
                case DragAction.Entered:
                    PlannerPreview_Create(HOUR_DP);
                    break;
                case DragAction.Exited:
                    // Remove the temporary RelativeLayout when leaving the drop zone.
                    PlannerPreview_Remove();
                    break;
                case DragAction.Drop:
                    // Remove the temporary RelativeLayout when the activity is dropped onto the planner.
                    PlannerPreview_Remove();

                    // Work out which hour the activity was dragged to in the UI.
                    int hour = CalculateHourFromPosition(dragEvent.GetY());

                    // Get the activity ID from the clip data.
                    int activityId = int.Parse(dragEvent.ClipData.GetItemAt(0).Text);

                    // Calculate the start and end times and create DateTime objects.
                    // End time is the same as start time plus one hour.
                    DateTime startTime = new DateTime(_ViewDate.Year, _ViewDate.Month, _ViewDate.Day, hour, 0, 0);
                    DateTime endTime = new DateTime(_ViewDate.Year, _ViewDate.Month, _ViewDate.Day, hour + 1, 0, 0);

                    // Add the activity to the backend.
                    AddActivityLogToBackend(activityId, startTime, endTime);

                    // Get all the activity logs from the backend again.
                    ReloadUI();
                    break;
                case DragAction.Location:
                    // The user has moved their finger. Move the preview.
                    PlannerPreview_Move(_RLHover, dragEvent.GetY());
                    break;
            }
        }

        private void ActivityLog_OnClick(object sender, EventArgs e)
        {
            var activityLogEditActivity = new Intent(this, typeof(ActivityLogEditActivity));

            // Get the ActivityLog ID from the sender and pass it to the Edit activity.
            activityLogEditActivity.PutExtra("activityLogId", int.Parse(((RelativeLayout)sender).GetTag(Resource.Id.activity_log_id).ToString()));

            StartActivity(activityLogEditActivity);
        }

        private void PreviousDay_OnClick(object sender, EventArgs e)
        {
            // Update the view date
            _ViewDate = _ViewDate.AddDays(-1);

            // Reload the UI.
            ReloadUI();
        }

        private void NextDay_OnClick(object sender, EventArgs e)
        {
            // Update the view date
            _ViewDate = _ViewDate.AddDays(1);

            // Reload the UI.
            ReloadUI();
        }
        #endregion

        #region planner
        private void ReloadUI()
        {
            // Set the heading to be the date.
            FindViewById<TextView>(Resource.Id.tvDate).Text = _ViewDate.Day + "/" + _ViewDate.Month + "/" + _ViewDate.Year;

            // Get the dropzone and attach a drag event listener.
            ClearDropzone();
            ClearActivities();
            GetActivities();
            GetActivityLogs();
        }

        private void PlannerPreview_Create(int size)
        {
            // This method creates a RelativeLayout that is meant to show the user where a new activity log will
            // be created if they drop an activity on the planner.
            _RLHover = new RelativeLayout(this);

            // Set the height of the temporary ImageView to 60dp. You have to use pixels, so calculate what 60dp is in pixels
            // https://developer.xamarin.com/recipes/android/resources/device_specific/detect_screen_size/
            float pixels = DPToPixels(size);

            _RLHover.SetBackgroundColor(new Android.Graphics.Color(GetColor(Resource.Color.blue_hover)));
            _RLHover.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, (int)pixels);

            _RLDropzone.AddView(_RLHover);
        }

        private void PlannerPreview_Remove()
        {
            // Remove the preview RelativeLayout from the drop zone.
            _RLDropzone.RemoveView(_RLHover);
        }

        private void PlannerPreview_Move(View view, float yPosition)
        {
            // This method positions a temporary RelativeLayout in the planner to show the user
            // where the activity log will be added. It snaps to the nearest hour on the planner.
            // An hour is 60dp, so just round to the previous 60dp.
            // Convert the Y position in pixels to dp.
            int dp = PixelsToDP(yPosition);

            // Divide the dp by 60 to determine which hour it is.
            int hour = (int)(Math.Floor((double)(dp / HOUR_DP)) * HOUR_DP);

            // Convert back to pixels.
            float pixels = DPToPixels(hour);

            // Move the RelativeLayout's position.
            view.SetY(pixels);
        }

        private void AddActivityLogToCalendar(ActivityLogModel activityLog)
        {
            //TODO: Verify the day here.
            // Work out where to place the start of the activity log.
            int startHour = activityLog.StartTime.Hour;

            // Calculate how big the View we're adding to the calendar will be.
            int hours = (int)activityLog.EndTime.Subtract(activityLog.StartTime).TotalHours;

            // Get the start position and height in pixels.
            float startPos = DPToPixels(startHour * HOUR_DP);

            // Remove 1 DP from the end of the height so it doesn't overlap with any items below it.
            float height = DPToPixels((hours * HOUR_DP) - 1);

            // Get the start and end times as strings.
            string startTime = activityLog.StartTime.ToString("HH:mm");
            string endTime = activityLog.EndTime.ToString("HH:mm");

            // Create the ActivityLog view and set the name and times in the TextView
            View vActivityLog = LayoutInflater.Inflate(Resource.Layout.ActivityLogEvent, null);
            vActivityLog.FindViewById<TextView>(Resource.Id.tvActivityName).Text = activityLog.Activity.ActivityName;
            vActivityLog.FindViewById<TextView>(Resource.Id.tvStartEndTimes).Text = startTime + " - " + endTime;

            // Get the color for thie activity.
            vActivityLog.SetBackgroundColor(GetActivityColor(activityLog.Activity.ActivityId));

            RelativeLayout.LayoutParams rlParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, (int)height);
            rlParameters.LeftMargin = 10;
            rlParameters.RightMargin = 10;
            vActivityLog.LayoutParameters = rlParameters;

            // Assign an ID to the newly created activity log view.
            vActivityLog.Id = View.GenerateViewId();

            // Set the logs position in the layout
            vActivityLog.SetY(startPos);

            // Add the OnClick event listener.
            vActivityLog.Click += ActivityLog_OnClick;

            // Add a tag to the ActivityLog to identify it.
            vActivityLog.SetTag(Resource.Id.activity_log_id, activityLog.ActivityLogId);

            _RLDropzone.AddView(vActivityLog);

            // Get the number of children elements in the Dropzone.
            // This is used to find any views that are next to this one and position them correctly.
            // Based on this stackoverflow post: http://stackoverflow.com/questions/6615723/getting-child-elements-from-linearlayout
            int childCount = _RLDropzone.ChildCount;
            RelativeLayout childView;
            int adjacentLogs = 0;
            int lastId = 0;

            for(int i = 0; i < childCount; i++)
            {
                // Get the child view from the dropzone.
                childView = (RelativeLayout)_RLDropzone.GetChildAt(i);

                if(childView.Id == vActivityLog.Id)
                {
                    // This is the ActivityLog view, skip it.
                    continue;
                }

                // Measure the view so we can determine it's height.
                childView.Measure(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);

                // Check to see if the new view will be positioned next to this one.
                float startChildY = childView.GetY();
                float endChildY = startChildY + childView.MeasuredHeight;

                // Check if these two elements pass through each other at all.
                if (startChildY <= startPos && endChildY >= startPos ||
                    startChildY >= startPos && startChildY <= (startPos + height))
                {
                    // These elements are next to each other.
                    // Get the layout parameters of the current childView.
                    RelativeLayout.LayoutParams parameters = (RelativeLayout.LayoutParams)childView.LayoutParameters;

                    // Disable any alignment rules on the view.
                    parameters.RemoveRule(LayoutRules.AlignParentLeft);
                    parameters.RemoveRule(LayoutRules.AlignParentRight);
                    parameters.RemoveRule(LayoutRules.RightOf);

                    if (adjacentLogs != 0)
                    {
                        // Assign this view to be to the right of the previous one.
                        parameters.AddRule(LayoutRules.RightOf, lastId);
                    }

                    // Keep a record of this ID so the next view can be placed next to it.
                    lastId = childView.Id;
                    adjacentLogs++;
                }
            }

            // Assign this newly added activityLog to the right of the last one and tell it to expand to the parent elements right.
            RelativeLayout.LayoutParams alParameters = (RelativeLayout.LayoutParams)vActivityLog.LayoutParameters;
            if (adjacentLogs == 0)
            {

                System.Diagnostics.Debug.WriteLine("No adjacent logs");
                // If there are no logs next to this one, make it use all the space.
                alParameters.AddRule(LayoutRules.AlignParentLeft);
                alParameters.AddRule(LayoutRules.AlignParentRight);
            }
            else
            {
                alParameters.AddRule(LayoutRules.RightOf, lastId);
                alParameters.AddRule(LayoutRules.AlignParentRight);
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

        private Color GetActivityColor(int activityId)
        {
            // This method will return a color resource ID. This is used to keep the colors consistent between each activity type
            // on the planner.
            Color activityColor = default(Color);

            // Check if this activity id has been assigned a color in the past.
            foreach(KeyValuePair<int, Color> actColor in _ActivityColors)
            {
                if(actColor.Key == activityId)
                {
                    // If this activity has already been assigned a color, return it.
                    return actColor.Value;
                }
            }

            // If no saved color was found, get the next available color from the color array.
            // Get the color array from colors.xml
            // Managing of typed arrays: https://developer.android.com/guide/topics/resources/more-resources.html#TypedArray
            var colorArray = Resources.ObtainTypedArray(Resource.Array.activitylog_colors);

            // Divide the number of assigned colors by the number of possible colors and keep a record of the remainder.
            // There are (at the moment) only 6 colors, this should mean that if all colors are assigned, it will start again and reuse colors.
            int remainder = _ActivityColors.Count % colorArray.Length();

            // Get the color from the color array
            activityColor = colorArray.GetColor(remainder, 0);
            _ActivityColors.Add(new KeyValuePair<int, Color>(activityId, activityColor));

            return activityColor;
        }

        private void ClearDropzone()
        {
            // Remove all views in the dropzone. Use this when refreshing.
            _RLDropzone.RemoveAllViews();
        }

        private void ClearActivities()
        {
            // Remove all views in the dropzone. Use this when refreshing.
            LinearLayout llActivities = FindViewById<LinearLayout>(Resource.Id.llActivities);
            llActivities.RemoveAllViews();
        }

        public int PixelsToDP(float pixels)
        {
            // Convert pixels to DP.
            // https://developer.xamarin.com/recipes/android/resources/device_specific/detect_screen_size/
            return (int)((pixels) / Resources.DisplayMetrics.Density);
        }

        public float DPToPixels(int dp)
        {
            // Convert DP to pixels. This is required as most (if not all) elements don't accept DP programmatically.
            // https://developer.xamarin.com/recipes/android/resources/device_specific/detect_screen_size/
            return (dp) * Resources.DisplayMetrics.Density;
        }

        private int CalculateHourFromPosition(float yPosition)
        {
            // This method takes a position on the screen in pixels and calculates what the hour would be on the planner.
            // Each hour uses 60dp of screen space which makes it trivial to calculate.
            int dp = PixelsToDP(yPosition);

            // Divide the dp by 60 to determine which hour it is.
            return (int)(Math.Floor((double)(dp / HOUR_DP)));
        }
        #endregion
    }
}
