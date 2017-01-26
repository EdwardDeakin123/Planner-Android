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
	public class PlannerDailyFragment : PlannerFragment
	{
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
            return inflater.Inflate(Resource.Layout.PlannerDaily, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // The view has been created, assign the event handlers.
            // Get the dropzone and attach a drag event listener.
            View.FindViewById<RelativeLayout>(Resource.Id.rlDropzone).Drag += DropZone_Drag;

            // Maybe move this elsewhere.
            //DrawHourMarkers();
        }


        /*public override void OnResume()
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
        #region backend
        #endregion

        #region event handlers
        protected override void Previous_OnClick(object sender, EventArgs e)
        {
            // Update the view date
            _ViewDate = _ViewDate.AddDays(-1);

            // Reload the UI.
            Refresh();
        }

        protected override void Next_OnClick(object sender, EventArgs e)
        {
            // Update the view date
            _ViewDate = _ViewDate.AddDays(1);

            // Reload the UI.
            Refresh();
        }

        protected override void Planner_OnRefresh(object sender, EventArgs e)
        {
            Refresh();

            // Mark the refresh as complete.
            _SwipeLayout.Refreshing = false;
        }

        protected override void Refresh()
        {
            // Clear the activities and any dropzones.
            ClearActivities();
            ClearDropzone();

            // Update the title above the planner.
            SetTitle(_ViewDate.Day + "/" + _ViewDate.Month + "/" + _ViewDate.Year);

            // Retrieve the activities and activity logs.
            GetActivities();

            // This is a daily view, so only get activity logs from today.
            GetActivityLogs(_ViewDate, _ViewDate);
        }
        #endregion

        #region planner
        #endregion
    }
}
