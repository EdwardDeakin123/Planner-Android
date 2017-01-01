using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;




using Android.Provider;
using Java.Util;
using System.Collections.Generic;
using System.Net;

namespace Front_End
{
	[Activity(Label = "Main", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		ImageButton imageB1;
		ImageButton imageB2;
		ImageButton imageB3;
		ImageButton imageB4;
		ImageButton imageB5;
		ImageButton imageB6;
		ImageButton imageB7;
		ImageButton imageB8;
		ImageButton imageB9;
		ImageButton imageB10;
		ImageButton imageB11;
		ImageButton imageB12;
		ImageButton imageB13;
		ImageButton imageB14;
		ImageButton imageB15;
		ImageButton imageB16;

        ImageView ivTemp;

        // This variable is used to skip the backend and just use test data.
        bool FakeData = false;

		protected override void OnCreate(Bundle bundle)
		{
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get UI elements out of the layout
			//hold = FindViewById<TextView>(Resource.Id.result11);

            /*
			imageB1 = FindViewById<ImageButton>(Resource.Id.myButton1);
			imageB2 = FindViewById<ImageButton>(Resource.Id.myButton2);
			imageB3 = FindViewById<ImageButton>(Resource.Id.myButton3);
			imageB4 = FindViewById<ImageButton>(Resource.Id.myButton4);
			imageB5 = FindViewById<ImageButton>(Resource.Id.myButton5);
			imageB6 = FindViewById<ImageButton>(Resource.Id.myButton6);
			imageB7 = FindViewById<ImageButton>(Resource.Id.myButton7);
			imageB8 = FindViewById<ImageButton>(Resource.Id.myButton8);
			imageB9 = FindViewById<ImageButton>(Resource.Id.myButton9);
			imageB10 = FindViewById<ImageButton>(Resource.Id.myButton10);
			imageB11 = FindViewById<ImageButton>(Resource.Id.myButton11);
			imageB12 = FindViewById<ImageButton>(Resource.Id.myButton12);
			imageB13 = FindViewById<ImageButton>(Resource.Id.myButton13);
			imageB14 = FindViewById<ImageButton>(Resource.Id.myButton14);
			imageB15 = FindViewById<ImageButton>(Resource.Id.myButton15);
			imageB16 = FindViewById<ImageButton>(Resource.Id.myButton16);
            */

            //var button1 = FindViewById<Button>(Resource.Id.button1);
			//var button2 = FindViewById<Button>(Resource.Id.button2);
            /*button1.LongClick += Button1_LongClick;
            button2.LongClick += Button2_LongClick;*/

            //button1.LongClick += Activity_LongClick;
            //button2.LongClick += Activity_LongClick;

            // Get the activity buttons from the backend
            GetActivities();
            GetActivityLogs();


            var dropZone = FindViewById<RelativeLayout>(Resource.Id.rlDropzone);

			/*imageB1.LongClick += IMButton1_LongClick;
			imageB2.LongClick += IMButton2_LongClick;
			imageB3.LongClick += IMButton3_LongClick;
			imageB4.LongClick += IMButton4_LongClick;
			imageB5.LongClick += IMButton5_LongClick;
			imageB6.LongClick += IMButton6_LongClick;
			imageB7.LongClick += IMButton7_LongClick;
			imageB8.LongClick += IMButton8_LongClick;
			imageB9.LongClick += IMButton9_LongClick;
			imageB10.LongClick += IMButton10_LongClick;
			imageB11.LongClick += IMButton11_LongClick;
			imageB12.LongClick += IMButton12_LongClick;
			imageB13.LongClick += IMButton13_LongClick;
			imageB14.LongClick += IMButton14_LongClick;
			imageB15.LongClick += IMButton15_LongClick;
			imageB16.LongClick += IMButton16_LongClick;*/

            dropZone.Drag += DropZone_Drop;
            /*dropZone2.Drag += DropZone_Drop;
            dropZone3.Drag += DropZone_Drop;
            dropZone4.Drag += DropZone_Drop;
            dropZone5.Drag += DropZone_Drop;
            dropZone6.Drag += DropZone_Drop;
            dropZone7.Drag += DropZone_Drop;*/


            base.OnCreate(bundle);
		}

        private async void GetActivities()
        {
            try
            {
                List<ActivityModel> activity;

                // Get activities from the backend.
                if (!FakeData)
                {
                    // Get real data.
                    BackendActivity backend = new BackendActivity();
                    activity = await backend.GetAll();
                }
                else
                {
                    // Populate the page with fake data.
                    activity = new List<ActivityModel>()
                    {
                        new ActivityModel() { ActivityId = 1, ActivityName = "Fake Running" },
                        new ActivityModel() { ActivityId = 2, ActivityName = "Fake Sleeping" }
                    };
                }

                foreach (ActivityModel act in activity)
                {
                    Button actButton = new Button(this);
                    actButton.Text = act.ActivityName;
                    actButton.LongClick += Activity_LongClick;

                    LinearLayout llActivities = FindViewById<LinearLayout>(Resource.Id.llActivities);
                    llActivities.AddView(actButton);
                    System.Diagnostics.Debug.WriteLine("This one is " + act.ActivityName);
                }
            }
            catch (System.Net.WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    // The user is not logged in. Move to the Login activity.
                    StartActivity(new Intent(this, typeof(LoginActivity)));
                }
                System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
            }
        }

        private async void GetActivityLogs()
        {
            try
            {
                List<ActivityLogModel> activityLogs;

                // Get activities from the backend.
                if (!FakeData)
                {
                    // Get real data.
                    BackendActivityLog backend = new BackendActivityLog();
                    activityLogs = await backend.GetByUser();
                }
                else
                {
                    // Populate the page with fake data.
                    activityLogs = new List<ActivityLogModel>()
                    {
                        new ActivityLogModel() { ActivityLogId = 1, Activity = new ActivityModel() { ActivityId = 1, ActivityName = "Sleeping Fake" }, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
                    };
                }

                foreach (ActivityLogModel actLog in activityLogs)
                {
                    Console.WriteLine("ActivityLog: " + actLog.Activity.ActivityName + " " + actLog.StartTime.ToString() + " " + actLog.EndTime.ToString());
                }
            }
            catch (System.Net.WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    // The user is not logged in. Move to the Login activity.
                    StartActivity(new Intent(this, typeof(LoginActivity)));
                }
                System.Diagnostics.Debug.WriteLine("Encountered an error while trying to connect to the server: " + ex.Message);
            }
        }

        /*
		void IMButton1_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "1");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}
        */

        /*
         *
         * CODE CLEANUP 
         * 
         */
        private void Activity_LongClick(object sender, View.LongClickEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Activity Long Click... Starting Drag...");

            // Get the Activity button from the sender object.
            Button activityButton = (Button)sender;

            // Create clip data that will be attached to the drag operation.
            var data = ClipData.NewPlainText("activity", activityButton.Text);

            // Start dragging and pass data
            // StartDrag was deprecated in API 24, check which version of Android we're running on and use the appropriate method.
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                activityButton.StartDragAndDrop(data, new View.DragShadowBuilder(activityButton), null, 0);
            }
            else
            {
                activityButton.StartDrag(data, new View.DragShadowBuilder(activityButton), null, 0);
            }
        }

        private void DropZone_Drop(object sender, View.DragEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Started dropping...");

            // Get the event from the sender object.
            var dragEvent = e.Event;

            RelativeLayout rLayout = (RelativeLayout)sender;

            // Perform an action.
            switch (dragEvent.Action)
            {
                case DragAction.Ended:
                case DragAction.Started:
                    e.Handled = true;
                    break;
                case DragAction.Entered:
                    System.Diagnostics.Debug.WriteLine("Entered this area.");

                    // Create an ImageView object that can be added to the drop zone as
                    // the button is dragged into it.
                    ivTemp = new ImageView(this);

                    // Set the height of the temporary ImageView to 60dp. You have to use pixels, so calculate what 60dp is in pixels
                    // https://developer.xamarin.com/recipes/android/resources/device_specific/detect_screen_size/
                    int pixels = (int)((60) * Resources.DisplayMetrics.Density);
                    ivTemp.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, pixels);

                    // Assign the image.
                    ivTemp.SetImageResource(Resource.Drawable.blue);

                    rLayout.AddView(ivTemp);
                    break;
                case DragAction.Exited:
                    System.Diagnostics.Debug.WriteLine("Exited this area.");

                    // Remove the temporary ImageView when leaving the drop zone.
                    rLayout.RemoveView(ivTemp);
                    break;
                case DragAction.Drop:
                    // Remove the temporary ImageView when leaving the drop zone.
                    rLayout.RemoveView(ivTemp);

                    break;
                case DragAction.Location:
                    // Make the ImageView follow the users finger but snap it to the closest hour.
                    // An hour is 60dp, so just round to the previous 60dp.
                    // Convert the Y position in pixels to dp.
                    int dp = (int)((dragEvent.GetY()) / Resources.DisplayMetrics.Density);

                    // Divide the dp by 60 to determine where to snap the hour.
                    int position = (int)(Math.Floor((double)(dp / 60)) * 60);

                    // Convert back to pixels.
                    float px = (position) * Resources.DisplayMetrics.Density;

                    ivTemp.SetY(px);
                    break;
            }
        }
        /*
         *
         * END CODE CLEANUP 
         * 
         */


        /*
        void DropZone_Drag(object sender, View.DragEventArgs e)
		{
			// React on different dragging events
			var evt = e.Event;
			switch (evt.Action)
			{
				case DragAction.Ended:
				case DragAction.Started:
					e.Handled = true;
					break;
				// Dragged element enters the drop zone
				case DragAction.Entered:

					if (T1.Text == "blue")
					{
						T1.Text = T1.Text;
					}
					else if (T1.Text == "red")
					{
						T1.Text = T1.Text;
					}

					else if (T1.Text == "blank")

						T1.Text = T1.Text;


					if (T1.Text == "red")
						imageB1.SetImageResource(Resource.Drawable.red);

					else if (T1.Text == "blue")
						imageB1.SetImageResource(Resource.Drawable.blue);


					if (T3.Text == T2.Text)

						T1.Text = T1.Text;

					else if (T2.Text == T1.Text)
					{
						imageB2.SetImageResource(Resource.Drawable.grey);
						T2.Text = "blank";
					}

					break;
				// Dragged element exits the drop zone
				case DragAction.Exited:

					if (T1.Text == "blue")
						imageB1.SetImageResource(Resource.Drawable.blueup);

					else if (T1.Text == "red")
						imageB1.SetImageResource(Resource.Drawable.redup);

					break;
				// Dragged element has been dropped at the drop zone
				case DragAction.Drop:
					// You can check if element may be dropped here
					// If not do not set e.Handled to true
					e.Handled = true;

					// Try to get clip data
					var data = e.Event.ClipData;
					if (data != null)



						color = data.GetItemAt(0).Text;
					

					if (color == "red")
					{
						imageB1.SetImageResource(Resource.Drawable.red);
						T1.Text = "red";
					}


					else if (color == "blue")
					{
						imageB1.SetImageResource(Resource.Drawable.blue);
						T1.Text = "blue";
					}
					else
					{
						T1.Text = "blank";
						imageB1.SetImageResource(Resource.Drawable.grey);
					}



					break;
			}
		}
        */
	}
}
