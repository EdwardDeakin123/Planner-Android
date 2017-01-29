using Android.App;
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
using Android.Support.V4.Widget;
using System.Xml.Serialization;

namespace Front_End
{
    [Activity(Label = "Activity Tracker")]
    public abstract class PlannerFragment : Fragment
    {
        //TODO Do more stuff in separate threads.
        // Layout elements.
        protected RelativeLayout _RLHover;
        protected SwipeRefreshLayout _SwipeLayout;

        // The date of the current view.
        protected DateTime _ViewDate;

        // Global variables used with the demo mode (_FakeData).
        protected List<ActivityModel> _Activities;
        protected List<ActivityLogModel> _ActivityLogs;

        // List that contains the list of colors associated with each activity ID.
        protected List<KeyValuePair<int, Color>> _ActivityColors;

        // Set the DP taken up by one hour.
        protected const int HOUR_DP = 60;

        // Use constants to differenciate between Daily and Weekly view types.
        public const int DAILY_VIEW = 0;
        public const int WEEKLY_VIEW = 1;

        private Preferences _Preferences;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Notify the calling activity that this fragment has an options menu.
            SetHasOptionsMenu(true);

            // Create an instance of Preferences. This will help to determine if this is running in demo mode, etc.
            // Demo mode will skip the backend and just use fake data.
            _Preferences = new Preferences();

            // Attempt to restore from the cache on the disk. This will recover the activities and logs from a previous run.
            RestoreFromCache();

            if (savedInstanceState != null)
            {
                // This is not a new instance of the fragment. Attempt to recover the view date.
                int year = savedInstanceState.GetInt("viewDateYear", -1);
                int month = savedInstanceState.GetInt("viewDateMonth", -1);
                int day = savedInstanceState.GetInt("viewDateDay", -1);

                // If any of the values return -1, just use todays date as the view date.
                if(year > -1 || month > -1 || day > -1)
                {
                    _ViewDate = new DateTime(year, month, day, 0, 0, 0);
                }
                else
                {
                    _ViewDate = DateTime.Now;
                }
            }

            // Check to see if a bundle was passed to this fragment.
            // This will happen when returning from ActivityLogEditFragment.
            if (this.Arguments != null)
            {
                // Retrieve the view date if it was set.
                int year = this.Arguments.GetInt("viewDateYear", -1);
                int month = this.Arguments.GetInt("viewDateMonth", -1);
                int day = this.Arguments.GetInt("viewDateDay", -1);

                // Make sure all of the values were set before updating the view date.
                if(year > -1 && month > -1 && day > -1)
                {
                    _ViewDate = new DateTime(year, month, day, 0, 0, 0);
                }

                // Attempt to get an updated activity log from the bundle. This will only happen in demo mode.
                string activityLogXml = this.Arguments.GetString("updatedActivityLog", "");

                // Make sure the activity log is set.
                if(activityLogXml != "")
                {
                    System.Diagnostics.Debug.WriteLine("Got an updated activity log!!!!!!! " + activityLogXml);

                    ActivityLogModel updatedActLog = Utility.DeserializeFromString<ActivityLogModel>(activityLogXml);

                    System.Diagnostics.Debug.WriteLine("About to remove it. ID: " + updatedActLog.ActivityLogId);
                    // Remove the current version of this activity log from list of activity logs.
                    _ActivityLogs.RemoveAll(actLog => actLog.ActivityLogId == updatedActLog.ActivityLogId);
                    System.Diagnostics.Debug.WriteLine("Removed, about to add.");


                    // Add the updated log into the list
                    _ActivityLogs.Add(updatedActLog);
                }
            }

            _ActivityColors = new List<KeyValuePair<int, Color>>();
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Assign the next and previous onclick events.
            View.FindViewById<ImageButton>(Resource.Id.ibNextDate).Click += Next_OnClick;
            View.FindViewById<ImageButton>(Resource.Id.ibPrevDate).Click += Previous_OnClick;

            _SwipeLayout = View.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            _SwipeLayout.Refresh += Planner_OnRefresh;

            DrawHourMarkers();
            DrawTimes();
            Refresh();

            // TODO Schedule this to update at regular intervals.
            DrawCurrentTime();
        }

        #region menu
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater menuInflater)
        {
            menuInflater.Inflate(Resource.Menu.menu_fragment_daily_planner, menu);

            base.OnCreateOptionsMenu(menu, menuInflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case Resource.Id.refresh:
                    // Refresh the planner.
                    Refresh();
                    break;
                case Resource.Id.today:
                    // Switch to todays date.
                    _ViewDate = DateTime.Now;
                    Refresh();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
        #endregion

        protected abstract void Planner_OnRefresh(object sender, EventArgs e);
        protected abstract void Previous_OnClick(object sender, EventArgs e);
        protected abstract void Next_OnClick(object sender, EventArgs e);
        protected abstract void Refresh();

        protected void SetTitle(string title)
        {
            // Set the title at the stop of the planner.
            View.FindViewById<TextView>(Resource.Id.tvDate).Text = title;
        }

        public override void OnResume()
        {
            base.OnResume();

            // Reload the planner when the activity is resumed.
            //Refresh();

            /* TODO Need to update this to use the newer permission schemes in Android.
            TimesDatabase timeDb = new TimesDatabase();

            if (timeDb.FindLatest() != DateTime.Now.Day)
            {
                new Notification();
            }
            */
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            System.Diagnostics.Debug.WriteLine("Saving Instance.");

            // When the instance is destroyed or recreated, save any important information for reuse.
            // This happens when the screen orientation changes.
            outState.PutInt("viewDateYear", _ViewDate.Year);
            outState.PutInt("viewDateMonth", _ViewDate.Month);
            outState.PutInt("viewDateDay", _ViewDate.Day);

            // Save the activities and logs to the disk.
            CacheToDisk();
        }

        public override void OnStop()
        {
            base.OnStop();

            System.Diagnostics.Debug.WriteLine("Stopping now...");

            // When the instance is stopped (for example, when changing from daily to weekly views), cache the activities and logs to disk.
            CacheToDisk();
        }

        #region UI
        protected void DrawHourMarkers()
        {
            System.Diagnostics.Debug.WriteLine("Drawing markers.");

            int currentPos = 0;
            RelativeLayout rlPlanner = View.FindViewById<RelativeLayout>(Resource.Id.rlPlanner);

            for (int i = 0; i < 24; i++)
            {
                currentPos += HOUR_DP;

                //System.Diagnostics.Debug.WriteLine("Putting a marker at " + currentPos);

                // Create a new horizontal divider.
                View hourMarker = new View(this.Activity);
                hourMarker.SetBackgroundColor(new Android.Graphics.Color(Activity.GetColor(Resource.Color.planner_divider)));

                // Get the layout and set the margins.
                RelativeLayout.LayoutParams rlParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, (int)DPToPixels(1));
                rlParameters.TopMargin = (int)DPToPixels(currentPos);
                rlParameters.LeftMargin = (int)DPToPixels(HOUR_DP);

                hourMarker.LayoutParameters = rlParameters;
                rlPlanner.AddView(hourMarker);
            }
        }

        protected void DrawTimes()
        {
            // Every time marker should be offset by half of the HOUR_DP so it is aligned with the hour marker.
            int currentPos = HOUR_DP / 2;
            RelativeLayout rlTimeMarkers = View.FindViewById<RelativeLayout>(Resource.Id.rlTimeMarkers);

            // Get all of the possible times from the strings file.
            string[] timesArray = Resources.GetStringArray(Resource.Array.times);

            foreach(string time in timesArray)
            {
                TextView tvTime = new TextView(this.Activity);

                // Set text for this TextView.
                tvTime.Text = time;
                tvTime.Gravity = GravityFlags.Center;
                tvTime.SetHeight((int)DPToPixels(60));

                // Create a new LayoutParams object to set the height, width, etc.
                RelativeLayout.LayoutParams rlParameters = new RelativeLayout.LayoutParams((int)DPToPixels(HOUR_DP), ViewGroup.LayoutParams.WrapContent);
                rlParameters.TopMargin = (int)DPToPixels(currentPos);
                tvTime.LayoutParameters = rlParameters;

                rlTimeMarkers.AddView(tvTime);
                currentPos += HOUR_DP;
            }
        }

        protected void DrawCurrentTime()
        {
            DateTime currentTime = DateTime.Now;
            RelativeLayout dropzone = default(RelativeLayout);

            if(GetPlannerMode() == DAILY_VIEW)
            {
                System.Diagnostics.Debug.WriteLine("CurrentTime: Daily View");

                if (!(_ViewDate.Day == currentTime.Day && _ViewDate.Month == currentTime.Month && _ViewDate.Year == currentTime.Year))
                {
                    // Today's date is not displayed on the current screen. Don't do anything.
                    return;
                }
                    System.Diagnostics.Debug.WriteLine("CurrentTime: Displaying today.");

                    // We are displaying today. Get the dropzone.
                    dropzone = View.FindViewById<RelativeLayout>(Resource.Id.rlDropzone);
            }
            else
            {
                // Get todays day of the week.
                int curDow = (int)currentTime.DayOfWeek;

                // Check if the current day of week is todays date in the current view.
                DateTime curDate = GetDateTimeForDayOfWeek(curDow);

                // Check if today's date is in the display.
                if (!(curDate.Day == currentTime.Day && curDate.Month == currentTime.Month && curDate.Year == currentTime.Year))
                {
                    // Today's date is not displayed on the current screen. Don't do anything.
                    return;
                }

                // Today's date is on display, get the correct dropzone.
                dropzone = GetDropzoneByDayOfWeek(curDow);
            }

            // Use the current time to determine where to place the "current time line".
            int hour = currentTime.Hour * HOUR_DP;
            // This calculation will cancel itself out if HOUR_DP is set to 60 (as default), decided to leave it in case the DP is changed later.
            int minutes = (int)((currentTime.Minute / 60.00F) * HOUR_DP);

            View timeMarker = new View(this.Activity);
            timeMarker.SetBackgroundColor(new Android.Graphics.Color(Activity.GetColor(Resource.Color.bright_red)));

            // Get the layout and set the margins.
            RelativeLayout.LayoutParams rlParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, (int)DPToPixels(2));
            rlParameters.TopMargin = (int)DPToPixels(hour + minutes);
            timeMarker.LayoutParameters = rlParameters;
            dropzone.AddView(timeMarker);
        }
        #endregion

        #region backend
        protected async void GetActivities()
        {
            try
            {
                // Store activities in a global variable so they can be referenced later without requiring another
                // connection to the backend.
                List<ActivityModel> activities = new List<ActivityModel>();

                // Get activities from the backend.
                if (!_Preferences.DemoMode)
                {
                    // Get real data.
                    BackendActivity backend = new BackendActivity();
                    _Activities = await backend.GetAll();

                    System.Diagnostics.Debug.WriteLine("Got " + _Activities.Count + " activities");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Running in dummy mode.");

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

                    Button actButton = new Button(this.Activity);
                    actButton.Text = act.ActivityName;

                    // Add the ID of this activity to the button's tag so it can later be identified.
                    actButton.SetTag(Resource.Id.activity_id, act.ActivityId);
                    actButton.LongClick += Activity_LongClick;

                    View.FindViewById<LinearLayout>(Resource.Id.llActivities).AddView(actButton);
                }
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    // The user is not logged in. Load the Login fragment.
                    Fragment fragment = new LoginFragment();
                    FragmentManager.BeginTransaction()
                        .Replace(Resource.Id.content, fragment)
                        .Commit();
                }

                System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
            }
            catch (BackendTimeoutException)
            {
                // Hit a timeout while trying to connect to the backend.
                DisplayAlert(GetString(Resource.String.timeout), GetString(Resource.String.timeout_message));
            }
            catch(UriFormatException)
            {
                // There is an error in the server address.
                DisplayAlert(GetString(Resource.String.server_address_error), GetString(Resource.String.server_address_error_message));
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
                                // The user is not logged in. Load the Login fragment.
                                Fragment fragment = new LoginFragment();
                                FragmentManager.BeginTransaction()
                                    .Replace(Resource.Id.content, fragment)
                                    .Commit();

                                // The exception has been handled. Return true.
                                return true;
                            }
                            else if (((HttpWebResponse)wEx.Response).StatusCode == HttpStatusCode.NotFound)
                            {
                                //TODO handle all other messages as Unexpected Error.
                                DisplayAlert(GetString(Resource.String.unexpected_error), GetString(Resource.String.unexpected_error_message));

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

        protected async void GetActivityLogs(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Get activities from the backend.
                if (!_Preferences.DemoMode)
                {
                    // Get real data.
                    BackendActivityLog backend = new BackendActivityLog();
                    _ActivityLogs = await backend.GetByDate(startDate, endDate);
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
                    // The user is not logged in. Load the Login fragment.
                    Fragment fragment = new LoginFragment();
                    FragmentManager.BeginTransaction()
                        .Replace(Resource.Id.content, fragment)
                        .Commit();
                }
                System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
            }
            catch (BackendTimeoutException)
            {
                // Display a popup.
                DisplayAlert(GetString(Resource.String.timeout), GetString(Resource.String.timeout_message));
            }
            catch (UriFormatException)
            {
                // There is an error in the server address.
                DisplayAlert(GetString(Resource.String.server_address_error), GetString(Resource.String.server_address_error_message));
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
                                // The user is not logged in. Load the Login fragment.
                                Fragment fragment = new LoginFragment();
                                FragmentManager.BeginTransaction()
                                    .Replace(Resource.Id.content, fragment)
                                    .Commit();

                                // The exception has been handled. Return true.
                                return true;
                            }
                            else if (((HttpWebResponse)wEx.Response).StatusCode == HttpStatusCode.NotFound)
                            {
                                DisplayAlert(GetString(Resource.String.unexpected_error), GetString(Resource.String.unexpected_error_message));

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

        protected async void AddActivityLogToBackend(int activityId, DateTime startTime, DateTime endTime)
        {
            try
            {
                if (!_Preferences.DemoMode)
                {
                    // Add the acitivity log to the backend.
                    BackendActivityLog backend = new BackendActivityLog();
                    await backend.Add(activityId, startTime, endTime);
                }
                else
                {
                    // We need to generate an activitylog id when creating fake entries.
                    // Use the number of activity logs in the list as the ID.
                    int activityLogId = _ActivityLogs.Count;

                    // Create an ActivityLog and add it to the global list but don't push it to the database.
                    // Get the activity associated with this log.
                    ActivityModel activity = _Activities.Where(act => act.ActivityId == activityId).FirstOrDefault();
                    _ActivityLogs.Add(new ActivityLogModel() { ActivityLogId = activityLogId, Activity = activity, StartTime = startTime, EndTime = endTime });
                }
            }
            catch (System.Net.WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    // The user is not logged in. Load the Login fragment.
                    Fragment fragment = new LoginFragment();
                    FragmentManager.BeginTransaction()
                        .Replace(Resource.Id.content, fragment)
                        .Commit();
                }
                System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
            }
            catch (BackendTimeoutException)
            {
                // Display a popup.
                DisplayAlert(GetString(Resource.String.timeout), GetString(Resource.String.timeout_message));
            }
            catch (UriFormatException)
            {
                // There is an error in the server address.
                DisplayAlert(GetString(Resource.String.server_address_error), GetString(Resource.String.server_address_error_message));
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
                                // The user is not logged in. Load the Login fragment.
                                Fragment fragment = new LoginFragment();
                                FragmentManager.BeginTransaction()
                                    .Replace(Resource.Id.content, fragment)
                                    .Commit();

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
        protected void Activity_LongClick(object sender, View.LongClickEventArgs e)
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

        protected void DropZone_Drag(object sender, View.DragEventArgs e)
        {
            // Get the event from the sender object.
            var dragEvent = e.Event;

            // Get the Dropzone's relative layout.
            RelativeLayout dropzone = (RelativeLayout)sender;

            // Perform an action.
            switch (dragEvent.Action)
            {
                case DragAction.Ended:
                case DragAction.Started:
                    e.Handled = true;
                    break;
                case DragAction.Entered:
                    PlannerPreview_Create(dropzone, HOUR_DP);
                    break;
                case DragAction.Exited:
                    // Remove the temporary RelativeLayout when leaving the drop zone.
                    PlannerPreview_Remove(dropzone);
                    break;
                case DragAction.Drop:
                    // Remove the temporary RelativeLayout when the activity is dropped onto the planner.
                    PlannerPreview_Remove(dropzone);

                    // Work out which hour the activity was dragged to in the UI.
                    int hour = CalculateHourFromPosition(dragEvent.GetY());

                    // Get the activity ID from the clip data.
                    int activityId = int.Parse(dragEvent.ClipData.GetItemAt(0).Text);

                    // Calculate the start and end date / times for this activity log.
                    DateTime startTime;
                    DateTime endTime;

                    // Determine if this is a daily or weekly view.
                    // TODO this can probably be set earlier and not done every time something is dropped.
                    if (GetPlannerMode() == DAILY_VIEW)
                    {
                        // This is a daily view. Calculate the start and end times and create DateTime objects.
                        // End time is the same as start time plus one hour.
                        startTime = new DateTime(_ViewDate.Year, _ViewDate.Month, _ViewDate.Day, hour, 0, 0);
                        endTime = new DateTime(_ViewDate.Year, _ViewDate.Month, _ViewDate.Day, hour + 1, 0, 0);

                        System.Diagnostics.Debug.WriteLine("This is a daily view.");
                    }
                    else
                    {
                        // This is a weekly view. ViewDate is not necessarily the correct date to use.
                        // Get the day of the week of this dropzone.
                        int dzDow = GetDayOfWeekByDropzone(dropzone);

                        // Get the date for the dropzone.
                        DateTime alDate = GetDateTimeForDayOfWeek(dzDow);

                        System.Diagnostics.Debug.WriteLine("This is a weekly view. View Date is the " + alDate.Day + " day of the month");

                        startTime = new DateTime(alDate.Year, alDate.Month, alDate.Day, hour, 0, 0);
                        endTime = new DateTime(alDate.Year, alDate.Month, alDate.Day, hour + 1, 0, 0);
                    }

                    // Add the activity to the backend.
                    AddActivityLogToBackend(activityId, startTime, endTime);

                    // Refresh the screen.
                    Refresh();
                    break;
                case DragAction.Location:
                    // The user has moved their finger. Move the preview.
                    PlannerPreview_Move(_RLHover, dragEvent.GetY());
                    break;
            }
        }

        protected void ActivityLog_OnClick(object sender, EventArgs e)
        {
            // Switch to the activity log edit fragment.
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            ActivityLogEditFragment activityLogEditFragment = new ActivityLogEditFragment();

            // Get the activityLogId from the tag on the sending object, then get the activity log from the list.
            int activityLogId = int.Parse(((RelativeLayout)sender).GetTag(Resource.Id.activity_log_id).ToString());
            //ActivityLogModel activityLog = _ActivityLogs.Where(actLog => actLog.ActivityLogId == activityLogId).FirstOrDefault();

            // Create a bundle that can be passed to the Edit Fragment.
            // Serialize the activity log so it can be sent and won't require another call to the backend to retrieve it.
            Bundle fragmentBundle = new Bundle();
            fragmentBundle.PutInt("activityLogId", activityLogId);

            // Tell the Edit fragment which planner sent it so it can return to the correct type.
            fragmentBundle.PutInt("plannerMode", GetPlannerMode());
            activityLogEditFragment.Arguments = fragmentBundle;
            fragmentTransaction.Replace(Resource.Id.content, activityLogEditFragment).Commit();
        }
        #endregion

        #region planner
        protected void PlannerPreview_Create(ViewGroup view, int size)
        {
            // This method creates a RelativeLayout that is meant to show the user where a new activity log will
            // be created if they drop an activity on the planner.
            _RLHover = new RelativeLayout(this.Activity);

            // Set the height of the temporary ImageView to 60dp. You have to use pixels, so calculate what 60dp is in pixels
            // https://developer.xamarin.com/recipes/android/resources/device_specific/detect_screen_size/
            float pixels = DPToPixels(size);

            _RLHover.SetBackgroundColor(new Android.Graphics.Color(Activity.GetColor(Resource.Color.blue_hover)));
            _RLHover.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, (int)pixels);

            view.AddView(_RLHover);
        }

        protected void PlannerPreview_Remove(ViewGroup view)
        {
            // Remove the preview RelativeLayout from the drop zone.
            view.RemoveView(_RLHover);
        }

        protected void PlannerPreview_Move(View view, float yPosition)
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

        protected void AddActivityLogToCalendar(ActivityLogModel activityLog)
        {
            //TODO Fix displaying multiple activity logs side by side in weekly view.
            // Determine which dropzone to place this activitylog into.
            RelativeLayout dropzone;

            if (GetPlannerMode() == DAILY_VIEW)
            {
                // Get the dropzone but also make sure that the the activity log should be displayed on this day.
                dropzone = View.FindViewById<RelativeLayout>(Resource.Id.rlDropzone);

                if(!(_ViewDate.Day == activityLog.StartTime.Day && _ViewDate.Month == activityLog.StartTime.Month && _ViewDate.Year == activityLog.StartTime.Year))
                {
                    // This is the incorrect day
                    return;
                }
            }
            else
            {
                // This is a weekly view, get the correct dropzone by the Day Of Week.
                dropzone = GetDropzoneByDayOfWeek((int)activityLog.StartTime.DayOfWeek);

                // Get the date for the day of the week this activity log falls on and check that it matches the date of the activity log.
                DateTime weekDate = GetDateTimeForDayOfWeek((int)activityLog.StartTime.DayOfWeek);

                if (!(weekDate.Day == activityLog.StartTime.Day && weekDate.Month == activityLog.StartTime.Month && weekDate.Year == activityLog.StartTime.Year))
                {
                    // This is the incorrect day
                    return;
                }
            }

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
            View vActivityLog = Activity.LayoutInflater.Inflate(Resource.Layout.ActivityLogEvent, null);
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
            dropzone.AddView(vActivityLog);

            // Get the number of children elements in the Dropzone.
            // This is used to find any views that are next to this one and position them correctly.
            // Based on this stackoverflow post: http://stackoverflow.com/questions/6615723/getting-child-elements-from-linearlayout
            int childCount = dropzone.ChildCount;
            RelativeLayout childView;
            int adjacentLogs = 0;
            int lastId = 0;

            for (int i = 0; i < childCount; i++)
            {
                // Make sure the child view is a relative layout.
                if(!(dropzone.GetChildAt(i) is RelativeLayout))
                    continue;

                // Get the child view from the dropzone.
                childView = (RelativeLayout)dropzone.GetChildAt(i);
                System.Diagnostics.Debug.WriteLine("22222");

                if (childView.Id == vActivityLog.Id)
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
        protected void DisplayAlert(string title, string message)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this.Activity);

            builder.SetMessage(message);
            builder.SetTitle(title);

            AlertDialog dialog = builder.Create();
            dialog.Create();
            dialog.Show();
        }

        protected RelativeLayout GetDropzoneByDayOfWeek(int dayOfWeek)
        {
            // Get the drop zone for any of the days of the week.
            RelativeLayout dropzone = default(RelativeLayout);

            switch (dayOfWeek)
            {
                case 0:
                    dropzone = View.FindViewById<RelativeLayout>(Resource.Id.rlSunDropzone);
                    break;
                case 1:
                    dropzone = View.FindViewById<RelativeLayout>(Resource.Id.rlMonDropzone);
                    break;
                case 2:
                    dropzone = View.FindViewById<RelativeLayout>(Resource.Id.rlTueDropzone);
                    break;
                case 3:
                    dropzone = View.FindViewById<RelativeLayout>(Resource.Id.rlWedDropzone);
                    break;
                case 4:
                    dropzone = View.FindViewById<RelativeLayout>(Resource.Id.rlThuDropzone);
                    break;
                case 5:
                    dropzone = View.FindViewById<RelativeLayout>(Resource.Id.rlFriDropzone);
                    break;
                case 6:
                    dropzone = View.FindViewById<RelativeLayout>(Resource.Id.rlSatDropzone);
                    break;
            }

            return dropzone;
        }

        protected int GetDayOfWeekByDropzone(RelativeLayout dropzone)
        {
            // Get the drop zone for any of the days of the week.
            int dow = -1;

            switch (dropzone.Id)
            {
                case Resource.Id.rlSunDropzone:
                    dow = 0;
                    break;
                case Resource.Id.rlMonDropzone:
                    dow = 1;
                    break;
                case Resource.Id.rlTueDropzone:
                    dow = 2;
                    break;
                case Resource.Id.rlWedDropzone:
                    dow = 3;
                    break;
                case Resource.Id.rlThuDropzone:
                    dow = 4;
                    break;
                case Resource.Id.rlFriDropzone:
                    dow = 5;
                    break;
                case Resource.Id.rlSatDropzone:
                    dow = 6;
                    break;
            }

            return dow;
        }

        protected DateTime GetDateTimeForDayOfWeek(int dow)
        {
            // Get the day of the week of the view date.
            int vdDow = (int)_ViewDate.DayOfWeek;

            // Check if the current day of week is todays date in the current view.
            // Calculate the difference between the view date and the current day so we can calculate the actual date.
            int diff = dow - vdDow;

            System.Diagnostics.Debug.WriteLine("Diff is: " + diff);

            // Add (or remove if negative) the difference from the view date.
            return _ViewDate.AddDays(diff);
        }

        protected int GetPlannerMode()
        {
            // Count how many dropzones are on the screen. This will tell us if it's a daily or weekly view.
            int noDropzones = View.FindViewById<LinearLayout>(Resource.Id.llDropzoneParent).ChildCount;

            if (noDropzones == 1)
            {
                // If there is only one child, this is a daily view.
                return DAILY_VIEW;
            }

            // Otherwise, this is a weekly view.
            return WEEKLY_VIEW;
        }

        protected Color GetActivityColor(int activityId)
        {
            // This method will return a color resource ID. This is used to keep the colors consistent between each activity type
            // on the planner.
            Color activityColor = default(Color);

            // Check if this activity id has been assigned a color in the past.
            foreach (KeyValuePair<int, Color> actColor in _ActivityColors)
            {
                if (actColor.Key == activityId)
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

        protected void ClearDropzone()
        {
            // Loop through all of the dropzones on the screen and remove all child elements.
            LinearLayout llDropzoneParent = View.FindViewById<LinearLayout>(Resource.Id.llDropzoneParent);

            int childCount = llDropzoneParent.ChildCount;
            RelativeLayout childView;

            for (int i = 0; i < childCount; i++)
            {
                // Get the child view from the dropzone.
                childView = (RelativeLayout)llDropzoneParent.GetChildAt(i);
                childView.RemoveAllViews();
            }
        }

        protected void ClearActivities()
        {
            // Remove all views in the dropzone. Use this when refreshing.
            LinearLayout llActivities = View.FindViewById<LinearLayout>(Resource.Id.llActivities);
            llActivities.RemoveAllViews();
        }

        protected int PixelsToDP(float pixels)
        {
            // Convert pixels to DP.
            // https://developer.xamarin.com/recipes/android/resources/device_specific/detect_screen_size/
            return (int)((pixels) / Resources.DisplayMetrics.Density);
        }

        protected float DPToPixels(int dp)
        {
            // Convert DP to pixels. This is required as most (if not all) elements don't accept DP programmatically.
            // https://developer.xamarin.com/recipes/android/resources/device_specific/detect_screen_size/
            return (dp) * Resources.DisplayMetrics.Density;
        }

        protected int CalculateHourFromPosition(float yPosition)
        {
            // This method takes a position on the screen in pixels and calculates what the hour would be on the planner.
            // Each hour uses 60dp of screen space which makes it trivial to calculate.
            int dp = PixelsToDP(yPosition);

            // Divide the dp by 60 to determine which hour it is.
            return (int)(Math.Floor((double)(dp / HOUR_DP)));
        }

        private void CacheToDisk()
        {
            // Serialize the Activities and Activity logs when the activity stops and save them to disk.
            // This can then be retrieved the next time this fragment runs. This allows us to pass data between the daily
            // and weekly views, reducing the number of calls to the backend. It also makes the dummy data mode better.

            // Add the activity logs and activities to an ObjectCacheModel object. This will help manage stale items in the cache.
            ObjectCache<List<ActivityModel>> activityCache = new ObjectCache<List<ActivityModel>>() { Object = _Activities };
            ObjectCache<List<ActivityLogModel>> activityLogCache = new ObjectCache<List<ActivityLogModel>>() { Object = _ActivityLogs };

            System.Diagnostics.Debug.WriteLine("Serializing objects now.");

            // Serialize these objects and write them to the disk.
            Utility.WriteToFile(Utility.SerializeToString(activityCache), "activities.txt");
            Utility.WriteToFile(Utility.SerializeToString(activityLogCache), "activitylogs.txt");
        }

        private void RestoreFromCache()
        {
            // Load the cached activities and logs from the disk.
            // Check to see if there are any cached versions of the data that can be retrieved from the disk.
            try
            {
                // Get the serialized data from the disk.
                string activityXml = Utility.ReadFromFile("activities.txt");
                ObjectCache<List<ActivityModel>> activityCache = Utility.DeserializeFromString<ObjectCache<List<ActivityModel>>>(activityXml);

                // Check if the cached object has expired, if it has - create a new object.
                // Otherwise return the cached object.
                if (activityCache.Expired)
                    _Activities = new List<ActivityModel>();
                else
                    _Activities = activityCache.Object;
            }
            catch
            {
                // There is no cache or an error occurred. Do nothing.
                _Activities = new List<ActivityModel>();
            }

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
        #endregion
    }
}
