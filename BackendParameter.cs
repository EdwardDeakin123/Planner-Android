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

namespace DragAndDropDemo
{
    // Helper class used when defining parameters that are passed to the backend.
    class BackendParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}