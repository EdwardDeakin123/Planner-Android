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

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            // The view has been created, assign the event handlers.
            // Assign the next and previous onclick events.
            View.FindViewById<ImageButton>(Resource.Id.ibNextDate).Click += NextDay_OnClick;
            View.FindViewById<ImageButton>(Resource.Id.ibPrevDate).Click += PreviousDay_OnClick;

            // Get the dropzone and attach a drag event listener.
            View.FindViewById<RelativeLayout>(Resource.Id.rlDropzone).Drag += DropZone_Drag;
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

        protected override void RefreshPlanner(object sender, EventArgs e)
        {
            ReloadUI();

            // Mark the refresh as complete.
            _SwipeLayout.Refreshing = false;
        }
        #endregion

        #region planner
        #endregion
    }
}
