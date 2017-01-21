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
        }

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
    }
}
