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

namespace Front_End
{
    [Activity(Label = "Activity Tracker")]
	public class PlannerWeeklyFragment : PlannerFragment
	{
        // TODO Swipe to move forward and backward a week at a time.
        // TODO Update the GetActivityLogs to accept a date range which can be controlled from either Weekly or Daily activities.
		public override void OnCreate(Bundle bundle)
		{
            base.OnCreate(bundle);

            if(_ViewDate == default(DateTime))
            {
                DateTime currentTime = DateTime.Now;

                // If the current view date is not set, set it to the current day.
                _ViewDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Return the layout to use in this fragment.
            return inflater.Inflate(Resource.Layout.PlannerWeekly, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // The view has been created, assign the event handlers.
            // Add the Drag event listener to the dropzones.
            View.FindViewById<RelativeLayout>(Resource.Id.rlSunDropzone).Drag += DropZone_Drag;
            View.FindViewById<RelativeLayout>(Resource.Id.rlMonDropzone).Drag += DropZone_Drag;
            View.FindViewById<RelativeLayout>(Resource.Id.rlTueDropzone).Drag += DropZone_Drag;
            View.FindViewById<RelativeLayout>(Resource.Id.rlWedDropzone).Drag += DropZone_Drag;
            View.FindViewById<RelativeLayout>(Resource.Id.rlThuDropzone).Drag += DropZone_Drag;
            View.FindViewById<RelativeLayout>(Resource.Id.rlFriDropzone).Drag += DropZone_Drag;
            View.FindViewById<RelativeLayout>(Resource.Id.rlSatDropzone).Drag += DropZone_Drag;
        }

        /*
        public override void OnResume()
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
        */

        private void SetDayOfMonth()
        {
            // Get the day of the week of the current view date.
            int dow = (int)_ViewDate.DayOfWeek;

            // Subtract dow from _View date to get the date on Sunday.
            DateTime sundayDate = _ViewDate.AddDays(-dow);
            DateTime mondayDate = sundayDate.AddDays(1);
            DateTime tuesdayDate = sundayDate.AddDays(2);
            DateTime wednesdayDate = sundayDate.AddDays(3);
            DateTime thursdayDate = sundayDate.AddDays(4);
            DateTime fridayDate = sundayDate.AddDays(5);
            DateTime saturdayDate = sundayDate.AddDays(6);

            // Update the day of month display.
            View.FindViewById<TextView>(Resource.Id.dayofmonth_sunday).Text = sundayDate.Day.ToString();
            View.FindViewById<TextView>(Resource.Id.dayofmonth_monday).Text = mondayDate.Day.ToString();
            View.FindViewById<TextView>(Resource.Id.dayofmonth_tuesday).Text = tuesdayDate.Day.ToString();
            View.FindViewById<TextView>(Resource.Id.dayofmonth_wednesday).Text = wednesdayDate.Day.ToString();
            View.FindViewById<TextView>(Resource.Id.dayofmonth_thursday).Text = thursdayDate.Day.ToString();
            View.FindViewById<TextView>(Resource.Id.dayofmonth_friday).Text = fridayDate.Day.ToString();
            View.FindViewById<TextView>(Resource.Id.dayofmonth_saturday).Text = saturdayDate.Day.ToString();
        }

        #region event handlers
        protected override void Planner_OnRefresh(object sender, EventArgs e)
        {
            Refresh();

            // Mark the refresh as complete.
            _SwipeLayout.Refreshing = false;
        }

        protected override void Previous_OnClick(object sender, EventArgs e)
        {
            // Update the view date
            _ViewDate = _ViewDate.AddDays(-7);

            // Reload the UI.
            Refresh();
        }

        protected override void Next_OnClick(object sender, EventArgs e)
        {
            // Update the view date.
            _ViewDate = _ViewDate.AddDays(7);

            // Reload the UI.
            Refresh();
        }

        protected override void Refresh()
        {
            System.Diagnostics.Debug.WriteLine("Refreshing...");

            // Get the date of Sunday this week.
            DateTime sundayDate = GetDateTimeForDayOfWeek(0);
            DateTime saturdayDate = GetDateTimeForDayOfWeek(6);

            // Retrieve the activities and activity logs.
            GetActivities();
            GetActivityLogs(sundayDate, saturdayDate);

            // Clear the activities and any dropzones.
            ClearActivities();
            ClearDropzone();

            // Update the title above the planner.

            SetTitle("Week of " + sundayDate.Day + "/" + sundayDate.Month + "/" + sundayDate.Year);

            // Update the day of the month displayed of the day of the week.
            SetDayOfMonth();
        }
        #endregion
    }
}
