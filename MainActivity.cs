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

using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Support.V7.AppCompat;

using Fragment = Android.App.Fragment;
using FragmentManager = Android.App.FragmentManager;

namespace Front_End
{
    [Activity(Label = "Activity Tracker", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
        // String array that contains the menu items.
        private static readonly string[] _NavigationItems = new string[] { "Daily", "Weekly", "Logout" };

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Navigation);

            System.Diagnostics.Debug.WriteLine("MainActivity is being created.");

            // Set up the navigation drawer.
            //string[] navigationArray = Resources.GetStringArray(Resource.Array.navigation_array);
            DrawerLayout navigationDraw = FindViewById<DrawerLayout>(Resource.Id.navigation_drawer);
            ListView navigationList = FindViewById<ListView>(Resource.Id.navigation_list);

            navigationList.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, _NavigationItems);
            navigationList.ItemClick += SelectItem;

            if (bundle == null)
            {
                // Show the Daily Planner by default.
                // Create an instance of the fragment.
                Fragment fragment = new PlannerDailyFragment();

                // Use the FragmentManager to assign the fragment to the content layout.
                //FragmentManager fragmentManager = this.FragmentManager;
                FragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content, fragment)
                    .Commit();
            }
        }

        private void SelectItem(object sender, AdapterView.ItemClickEventArgs e)
        {
            Fragment fragment = null;

            switch (e.Position)
            {
                case 0:
                    fragment = new PlannerDailyFragment();
                    break;
                case 1:
                    fragment = new PlannerWeeklyFragment();
                    break;
                case 2:
                    // Do the logout stuff here.
                    break;
            }

            if (fragment != null)
            {
                // Load the requested fragment.
                FragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content, fragment)
                    .Commit();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //MenuInflater.Inflate(Resource.Menu.menu_fragment_daily_planner, menu);

            //MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }
    }
}
