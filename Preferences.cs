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
using System.Net;

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
                // Return 52029 (Default IIS port with Visual Studio) if it's not been set already.
                return _SharedPreferences.GetInt("server_port", 52029);
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

        public Cookie AuthenticationCookie
        {
            get
            {
                string cookieName = _SharedPreferences.GetString("cookieName", "");
                string cookieDomain = _SharedPreferences.GetString("cookieDomain", "");
                string cookieValue = _SharedPreferences.GetString("cookieValue", "");
                string cookiePath = _SharedPreferences.GetString("cookiePath", "");

                if (cookieName != "")
                {
                    // If the cookieName is not empty, assume the other values were correctly retrieved.
                    // Create a cookie.
                    Cookie newCookie = new Cookie(cookieName, cookieValue, cookiePath, cookieDomain);


                    if (!newCookie.Expired)
                    {
                        // Add the cookie to the collection.
                        return newCookie;
                    }
                }
                
                // The cookie doesn't exist or has expired, return a default cookie.
                return default(Cookie);
            }
            set
            {
                // Edit the shared preferences.
                ISharedPreferencesEditor spEditor = _SharedPreferences.Edit();

                if (value == default(Cookie))
                {
                    // If a default cookie object is passed, remove the saved cookie.
                    spEditor.PutString("cookieName", "");
                    spEditor.PutString("cookieDomain", "");
                    spEditor.PutString("cookieValue", "");
                    spEditor.PutString("cookiePath", "");
                }
                else
                {
                    // Save the setting and commit it.
                    spEditor.PutString("cookieName", value.Name);
                    spEditor.PutString("cookieDomain", value.Domain);
                    spEditor.PutString("cookieValue", value.Value);
                    spEditor.PutString("cookiePath", value.Path);
                }

                spEditor.Apply();
            }
        }
    }
}