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
using System.Threading.Tasks;

namespace DragAndDropDemo
{
    // This is a helper class which manages API calls to the backend related to Users.
    class BackendUser : Backend
    {
        public BackendUser() : base()
        {
            _Resource = "User";
        }

        public async Task Login(string username, string password)
        {
            _Command = "Login";

            //TODO Work out a way to determine if the login was successful. (Status codes).
            // Set the parameters and post the login request.
            _Parameters.Add(new BackendParameter { Key = "username", Value = username });
            _Parameters.Add(new BackendParameter { Key = "password", Value = password });

            await PostRequestAsync();
        }

        public void Logout()
        {
            _Command = "Logout";

            PostRequestAsync();
        }

        public void Register(string firstname, string lastname, string username, string password)
        {
            _Command = "Register";

            // Set the parameters and post the login request.
            _Parameters.Add(new BackendParameter { Key = "firstname", Value = firstname });
            _Parameters.Add(new BackendParameter { Key = "lastname", Value = lastname });
            _Parameters.Add(new BackendParameter { Key = "username", Value = username });
            _Parameters.Add(new BackendParameter { Key = "password", Value = password });

            PostRequestAsync();
        }
    }
}