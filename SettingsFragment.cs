using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Preferences;
using Android.Support.V7.Widget;

namespace Front_End
{
    public class SettingsFragment : Fragment
    {
        Preferences _Preferences;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // The settings for the app are stored in the Shared Preferences.
            // Create an instance of the helper class I wrote to make it easier to manage.
            _Preferences = new Preferences();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Update the UI with data from shared preferences.
            GetSavedSettings();

            View.FindViewById<Switch>(Resource.Id.swDemoMode).CheckedChange += OnCheckedChange;
        }

        #region event handlers
        public void OnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            // Save the value of IsChecked in the shared preferences. This will enable or disable demo mode.
            _Preferences.DemoMode = e.IsChecked;
        }
        #endregion

        #region UI
        private void GetSavedSettings()
        {
            // Update the UI elements.
            View.FindViewById<Switch>(Resource.Id.swDemoMode).Checked = _Preferences.DemoMode;
        }
        #endregion
    }
}