using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Preferences;

namespace Front_End
{
    // This class is meant to manage all the of the preferences stored in Android's shared preferences.
    class Preferences
    {
        ISharedPreferences _SharedPreferences;

        public Preferences()
        {
            _SharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
        }

        public bool DemoMode
        {
            get
            {
                return _SharedPreferences.GetBoolean("demo_mode", false);
            }
            set
            {
                // Edit the shared preferences.
                ISharedPreferencesEditor spEditor = _SharedPreferences.Edit();

                // Save the setting and commit it.
                spEditor.PutBoolean("demo_mode", value);
                spEditor.Apply();
            }
        }

        public string ServerAddress
        {
            get
            {
                return _SharedPreferences.GetString("server_address", "");
            }
            set
            {
                // Edit the shared preferences.
                ISharedPreferencesEditor spEditor = _SharedPreferences.Edit();

                // Save the setting and commit it.
                spEditor.PutString("server_address", value);
                spEditor.Apply();
            }
        }

        public int ServerPort
        {
            get
            {
                return _SharedPreferences.GetInt("server_port", -1);
            }
            set
            {
                // Edit the shared preferences.
                ISharedPreferencesEditor spEditor = _SharedPreferences.Edit();

                // Save the setting and commit it.
                spEditor.PutInt("server_port", value);
                spEditor.Apply();
            }
        }
    }
}