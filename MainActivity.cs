using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Front_End
{
    [Activity(Label = "Main", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
        RelativeLayout _RLHover;
        DateTime _ViewDate = DateTime.Now;
        List<ActivityModel> _Activities;
        List<ActivityLogModel> _ActivityLogs;

        // This variable is used to skip the backend and just use test data.
        bool FakeData = false;

		protected override void OnCreate(Bundle bundle)
		{
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

            // Using a global variable so we can simulate adding data when using the FakeData switch.
            _ActivityLogs = new List<ActivityLogModel>();

            // Get the activity buttons from the backend
            GetActivities();
            GetActivityLogs();

            // Get the dropzone and attach a drag event listener.
            RelativeLayout dropZone = FindViewById<RelativeLayout>(Resource.Id.rlDropzone);
            dropZone.Drag += DropZone_Drop;

            base.OnCreate(bundle);
		}

        private async void GetActivities()
        {
            try
            {
                // Store activities in a global variable so they can be referenced later without requiring another
                // connection to the backend.
                List<ActivityModel> _Activities;

                // Get activities from the backend.
                if (!FakeData)
                {
                    // Get real data.
                    BackendActivity backend = new BackendActivity();
                    _Activities = await backend.GetAll();
                }
                else
                {
                    // Populate the page with fake data.
                    _Activities = new List<ActivityModel>()
                    {
                        new ActivityModel() { ActivityId = 1, ActivityName = "Fake Running" },
                        new ActivityModel() { ActivityId = 2, ActivityName = "Fake Sleeping" }
                    };
                }

                foreach (ActivityModel act in _Activities)
                {
                    Button actButton = new Button(this);
                    actButton.Text = act.ActivityName;

                    // Add the ID of this activity to the button's tag so it can later be identified.
                    actButton.SetTag(Resource.Id.activity_id, act.ActivityId);
                    actButton.LongClick += Activity_LongClick;

                    LinearLayout llActivities = FindViewById<LinearLayout>(Resource.Id.llActivities);
                    llActivities.AddView(actButton);
                    System.Diagnostics.Debug.WriteLine("This one is " + act.ActivityName);
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
        }

        private async void GetActivityLogs()
        {
            try
            {
                // Clear the dropzone before adding activity logs
                ClearDropzone();

                // Get activities from the backend.
                if (!FakeData)
                {
                    // Get real data.
                    BackendActivityLog backend = new BackendActivityLog();
                    _ActivityLogs = await backend.GetByUser();
                }

                foreach (ActivityLogModel actLog in _ActivityLogs)
                {
                    Console.WriteLine("ActivityLog: " + actLog.Activity.ActivityName + " " + actLog.StartTime.ToString() + " " + actLog.EndTime.ToString());

                    // Display the Activity Log on the calendar.
                    AddActivityLogToCalendar(actLog);
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
        }

        private async void AddActivityLogToBackend(int activityId, DateTime startTime, DateTime endTime)
        {
            try
            {
                if (!FakeData)
                {
                    // Get real data.
                    BackendActivityLog backend = new BackendActivityLog();
                    await backend.Add(activityId, startTime, endTime);
                }
                else
                {
                    //activityLogs = new List<ActivityLogModel>();
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
        }

        private void Activity_LongClick(object sender, View.LongClickEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Activity Long Click... Starting Drag...");

            // Get the Activity button from the sender object.
            Button activityButton = (Button)sender;

            // Create clip data that will be attached to the drag operation.
            // Get the activityId from the Tag and assign it to the clip data.
            var data = ClipData.NewPlainText("activityId", activityButton.GetTag(Resource.Id.activity_id).ToString());
            //var data = ClipData.NewPlainText("activityId", "1");

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

        private void DropZone_Drop(object sender, View.DragEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Started dropping...");
            float pixels;
            int dp;
            int hour;

            // Get the event from the sender object.
            var dragEvent = e.Event;

            // Get the dropzone from the sender object.
            RelativeLayout rLayout = (RelativeLayout)sender;

            // Perform an action.
            switch (dragEvent.Action)
            {
                case DragAction.Ended:
                case DragAction.Started:
                    e.Handled = true;
                    break;
                case DragAction.Entered:
                    // Create a RelativeLayout object that can be added to the drop zone as
                    // the button is dragged into it.
                    _RLHover = new RelativeLayout(this);

                    // Set the height of the temporary ImageView to 60dp. You have to use pixels, so calculate what 60dp is in pixels
                    // https://developer.xamarin.com/recipes/android/resources/device_specific/detect_screen_size/
                    pixels = DPToPixels(60); //(60) * Resources.DisplayMetrics.Density;

                    _RLHover.SetBackgroundColor(new Android.Graphics.Color(GetColor(Resource.Color.blue_hover)));
                    _RLHover.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, (int)pixels);

                    rLayout.AddView(_RLHover);
                    break;
                case DragAction.Exited:
                    // Remove the temporary RelativeLayout when leaving the drop zone.
                    rLayout.RemoveView(_RLHover);
                    break;
                case DragAction.Drop:
                    // Remove the temporary RelativeLayout when the activity is dropped.
                    rLayout.RemoveView(_RLHover);

                    // Calculate the hour based on the position of the users finger. An hour is equal to 60dp.
                    // Convert the Y position in pixels to dp.
                    dp = PixelsToDP(dragEvent.GetY()); //(int)((dragEvent.GetY()) / Resources.DisplayMetrics.Density);

                    // Divide the dp by 60 to determine which hour it is.
                    hour = (int)(Math.Floor((double)(dp / 60)));

                    // Get the activity ID from the clip data.
                    int activityId = int.Parse(dragEvent.ClipData.GetItemAt(0).Text);

                    // Calculate the start and end times and create DateTime objects.
                    // End time is the same as start time plus one hour.
                    DateTime startTime = new DateTime(_ViewDate.Year, _ViewDate.Month, _ViewDate.Day, hour, 0, 0);
                    DateTime endTime = new DateTime(_ViewDate.Year, _ViewDate.Month, _ViewDate.Day, hour + 1, 0, 0);

                    // Add the activity to the backend.
                    AddActivityLogToBackend(activityId, startTime, endTime);

                    // Get all the activity logs from the backend again.
                    GetActivityLogs();
                    break;
                case DragAction.Location:
                    // Make the ImageView follow the users finger but snap it to the closest hour.
                    // An hour is 60dp, so just round to the previous 60dp.
                    // Convert the Y position in pixels to dp.
                    dp = PixelsToDP(dragEvent.GetY()); //(int)((dragEvent.GetY()) / Resources.DisplayMetrics.Density);

                    // Divide the dp by 60 to determine which hour it is.
                    hour = (int)(Math.Floor((double)(dp / 60)) * 60);

                    // Convert back to pixels.
                    pixels = DPToPixels(hour); // (hour) * Resources.DisplayMetrics.Density;

                    _RLHover.SetY(pixels);
                    break;
            }
        }

        private void AddActivityLogToCalendar(ActivityLogModel activityLog)
        {
            //TODO: Verify the day here.
            // Work out where to place the start of the activity log.
            int startHour = activityLog.StartTime.Hour;

            // Calculate how big the View we're adding to the calendar will be.
            int hours = (int)activityLog.EndTime.Subtract(activityLog.StartTime).TotalHours;

            // Get the start position and height in pixels.
            float startPos = DPToPixels(startHour * 60);
            float height = DPToPixels(hours * 60);

            // Create a RelativeLayout the will represent the activity log in the planner.
            RelativeLayout rlDropzone = FindViewById<RelativeLayout>(Resource.Id.rlDropzone);
            RelativeLayout rlActivityLog = new RelativeLayout(this);
            TextView tvActivityName = new TextView(this);

            rlActivityLog.SetBackgroundColor(new Android.Graphics.Color(GetColor(Resource.Color.blue_hover)));
            rlActivityLog.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, (int)height);

            // Set the logs position in the layout
            rlActivityLog.SetY(startPos);

            // Display the activity name.
            tvActivityName.Text = activityLog.Activity.ActivityName;

            rlActivityLog.AddView(tvActivityName);
            rlDropzone.AddView(rlActivityLog);

            System.Diagnostics.Debug.WriteLine("This log starts at " + startHour + " hours");
            System.Diagnostics.Debug.WriteLine("This log goes for " + hours + " hours");
        }

        private void ClearDropzone()
        {
            // Remove all views in the dropzone. Use this when refreshing.
            RelativeLayout rlDropzone = FindViewById<RelativeLayout>(Resource.Id.rlDropzone);
            rlDropzone.RemoveAllViews();
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
    }
}
