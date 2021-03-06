﻿using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Net;
using Front_End.Models;
using Front_End.Backend;

using Fragment = Android.App.Fragment;
using FragmentManager = Android.App.FragmentManager;

namespace Front_End
{
    [Activity(Label = "Activity Tracker", MainLauncher = true, Icon = "@drawable/ic_today_white_24dp")]
	public class MainActivity : Activity
	{
        // String array that contains the menu items.
        private Preferences _Preferences;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Navigation);

            // Create an instance of Preferences which will manage the shared preferences in Android.
            _Preferences = new Preferences();

            // Set up the navigation drawer.
            CreateNavigationMenu();

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

        private void CreateNavigationMenu()
        {
            // Create the menu items.
            List<MenuItemModel> menuItems = new List<MenuItemModel>();

            menuItems.Add(new MenuItemModel { Text = GetString(Resource.String.daily), ImageResource = Resource.Drawable.ic_view_day_white_24dp });
            menuItems.Add(new MenuItemModel { Text = GetString(Resource.String.weekly), ImageResource = Resource.Drawable.ic_view_week_white_24dp });
            menuItems.Add(new MenuItemModel { Text = GetString(Resource.String.settings), ImageResource = Resource.Drawable.ic_settings_white_24dp });
            menuItems.Add(new MenuItemModel { Text = GetString(Resource.String.logout), ImageResource = Resource.Drawable.ic_exit_to_app_white_24dp });

            // Create the adapter and attach it to the menu and attach the item click event.
            ListView navigationList = FindViewById<ListView>(Resource.Id.navigation_list);

            navigationList.Adapter = new MenuAdapter(this, menuItems);
            navigationList.ItemClick += SelectItem;
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
                    fragment = new SettingsFragment();
                    break;
                case 3:
                    //TODO logout here
                    if (!_Preferences.DemoMode)
                    {
                        // Make sure we're not running in demo mode, then logout and redirect to the login fragment.
                        Logout();

                        // Destroy the local cookie.
                        _Preferences.AuthenticationCookie = default(Cookie);
                        fragment = new LoginFragment();
                    }
                    break;
            }

            if (fragment != null)
            {
                // Load the requested fragment.
                FragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content, fragment)
                    .AddToBackStack(null)
                    .Commit();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }

        #region backend
        private async void Logout()
        {
            try
            {
                // Prepare the User backend object.
                BackendUser backend = new BackendUser();
                await backend.Logout();
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
            }
        }
        #endregion
    }
}
