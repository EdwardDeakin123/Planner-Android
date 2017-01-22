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

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            // The view has been created, assign the event handlers.
            // Assign the next and previous onclick events.
            View.FindViewById<ImageButton>(Resource.Id.ibNextDate).Click += NextDay_OnClick;
            View.FindViewById<ImageButton>(Resource.Id.ibPrevDate).Click += PreviousDay_OnClick;

            // Add the Drag event listener to the dropzones.
            View.FindViewById<RelativeLayout>(Resource.Id.rlSunDropzone).Drag += DropZone_Drag;
            View.FindViewById<RelativeLayout>(Resource.Id.rlMonDropzone).Drag += DropZone_Drag;
            View.FindViewById<RelativeLayout>(Resource.Id.rlTueDropzone).Drag += DropZone_Drag;
            View.FindViewById<RelativeLayout>(Resource.Id.rlWedDropzone).Drag += DropZone_Drag;
            View.FindViewById<RelativeLayout>(Resource.Id.rlThuDropzone).Drag += DropZone_Drag;
            View.FindViewById<RelativeLayout>(Resource.Id.rlFriDropzone).Drag += DropZone_Drag;
            View.FindViewById<RelativeLayout>(Resource.Id.rlSatDropzone).Drag += DropZone_Drag;

            // Update the text views that show the date above each day.
            SetDayOfMonth();
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

        protected override void RefreshPlanner(object sender, EventArgs e)
        {
            ReloadUI();

            // Mark the refresh as complete.
            _SwipeLayout.Refreshing = false;
        }
    }
}
