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
    [Activity(Label = "Activity Tracker", MainLauncher = true, Icon = "@drawable/icon")]
	public class PlannerWeeklyActivity : PlannerActivity
	{
        // TODO Swipe to move forward and backward a week at a time.
		protected override void OnCreate(Bundle bundle)
		{
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.PlannerWeekly);

            base.OnCreate(bundle);
	    
            // Using a global variable so we can simulate adding data when using the FakeData switch.
            _Activities = new List<ActivityModel>();
            _ActivityLogs = new List<ActivityLogModel>();
            _ActivityLogId = 0;

            // Assign the next and previous onclick events.
            FindViewById<ImageButton>(Resource.Id.ibNextDate).Click += NextDay_OnClick;
            FindViewById<ImageButton>(Resource.Id.ibPrevDate).Click += PreviousDay_OnClick;

            if(_ViewDate == default(DateTime))
            {
                DateTime currentTime = DateTime.Now;

                // If the current view date is not set, set it to the current day.
                _ViewDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);
            }

            // Add the Drag event listener to the dropzones.
            FindViewById<RelativeLayout>(Resource.Id.rlSunDropzone).Drag += DropZone_Drag;
            FindViewById<RelativeLayout>(Resource.Id.rlMonDropzone).Drag += DropZone_Drag;
            FindViewById<RelativeLayout>(Resource.Id.rlTueDropzone).Drag += DropZone_Drag;
            FindViewById<RelativeLayout>(Resource.Id.rlWedDropzone).Drag += DropZone_Drag;
            FindViewById<RelativeLayout>(Resource.Id.rlThuDropzone).Drag += DropZone_Drag;
            FindViewById<RelativeLayout>(Resource.Id.rlFriDropzone).Drag += DropZone_Drag;
            FindViewById<RelativeLayout>(Resource.Id.rlSatDropzone).Drag += DropZone_Drag;
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
    }
}
