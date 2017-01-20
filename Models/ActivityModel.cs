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
using Front_End.Backend;

namespace Front_End.Models
{
    public class ActivityModel : IBackendType
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
    }
}