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
	public class PlannerDailyActivity : PlannerActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.PlannerDaily);

            base.OnCreate(bundle);
	    
            // Using a global variable so we can simulate adding data when using the FakeData switch.
            //_Activities = new List<ActivityModel>();
            //_ActivityLogs = new List<ActivityLogModel>();
            //_ActivityLogId = 0;
            //_ActivityColors = new List<KeyValuePair<int, Color>>();

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
            FindViewById<RelativeLayout>(Resource.Id.rlDropzone).Drag += DropZone_Drag;
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
        #endregion

        #region event handlers
        // TODO: Make these abstract, rename to Next and Previous 
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
        #endregion
    }
}
