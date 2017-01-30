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

namespace Front_End
{
    // Helper class used to keep track of space used on the planner when positioning elements.
    class PixelRange
    {
        public int Start { get; set; }
        public int End { get; set; }
    }
}