using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace DragAndDropDemo
{
	[Activity(Label = "Drag and Drop")]
	public class Activity1 : Activity
	{

		TextView hold;
		string color;
		TextView T1;
		TextView T2;
		TextView T3;
		TextView T4;

		ImageButton imageB1;
		ImageButton imageB21;
		ImageButton imageB4;

		ImageView image4;
		protected override void OnCreate(Bundle bundle)
		{
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get UI elements out of the layout
			hold = FindViewById<TextView>(Resource.Id.result11);

			T1 = FindViewById<TextView>(Resource.Id.T1);
			T2 = FindViewById<TextView>(Resource.Id.T2);
			T3 = FindViewById<TextView>(Resource.Id.T3);
			T4 = FindViewById<TextView>(Resource.Id.T4);

			imageB1 = FindViewById<ImageButton>(Resource.Id.myButton1);
			imageB21 = FindViewById<ImageButton>(Resource.Id.myButton2);

			imageB4 = FindViewById<ImageButton>(Resource.Id.myButton4);



			image4 = FindViewById<ImageView>(Resource.Id.image4);

			var button1 = FindViewById<Button>(Resource.Id.button1);
			button1.LongClick += Button1_LongClick;
			var button2 = FindViewById<Button>(Resource.Id.button2);
			button2.LongClick += Button2_LongClick;
			var dropZone1 = FindViewById<RelativeLayout>(Resource.Id.dropz);
			var dropZone2 = FindViewById<RelativeLayout>(Resource.Id.dropz1);
			var dropZone3 = FindViewById<RelativeLayout>(Resource.Id.dropz2);
			var dropZone4 = FindViewById<RelativeLayout>(Resource.Id.dropz3);

			//var button3 = FindViewById<ImageButton>(Resource.Id.myButton2);
			imageB1.LongClick += Button3_LongClick;
			//var button4 = FindViewById<ImageButton>(Resource.Id.myButton4);
			imageB21.LongClick += Button4_LongClick;



			// Attach event to drop zone
			dropZone1.Drag += DropZone_Drag;
			dropZone2.Drag += DropZone_Drag2;
			dropZone3.Drag += DropZone_Drag3;
			dropZone4.Drag += DropZone_Drag4;

			base.OnCreate(bundle);
		}

		void Button3_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "Element 3");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}

		void Button4_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "Element 4");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}

		void Button1_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "Element 1");

			// Start dragging and pass data
			((sender) as Button).StartDrag(data, new View.DragShadowBuilder(((sender) as Button)), null, 0);
		}

		void Button2_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "Element 2");

			// Start dragging and pass data
			((sender) as Button).StartDrag(data, new View.DragShadowBuilder(((sender) as Button)), null, 0);
			((sender) as Button).StartDrag(data, new View.DragShadowBuilder(((sender) as Button)), null, 0);
		}

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



					if (T1.Text == "red")
						imageB1.SetImageResource(Resource.Drawable.red);

					else if (T1.Text == "blue")
							imageB1.SetImageResource(Resource.Drawable.blue);

				/*else if (T1.Text == T2.Text)
					{
						imageB21.SetImageResource(Resource.Drawable.grey);
						T2.Text = "blank";*/
					

					if (T3.Text == T2.Text)

						T1.Text = T1.Text;

				else if (T2.Text == T1.Text)
					{
						imageB21.SetImageResource(Resource.Drawable.grey);
						T2.Text = "blank";
					}

					break;
				// Dragged element exits the drop zone
				case DragAction.Exited:


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
					/*if (T1.Text == "red")
					{
						imageB1.SetImageResource(Resource.Drawable.grey);
						T1.Text = "blank";
					}

					 if (T1.Text == "blue")
					{
						imageB1.SetImageResource(Resource.Drawable.grey);
						T1.Text = "blank";
					}*/

					/*if (T1.Text != "blank")
					{
						T1.Text = "blank";
						imageB1.SetImageResource(Resource.Drawable.grey);
					}*/

					if (color == "Element 1")
					{
						imageB1.SetImageResource(Resource.Drawable.red);
						T1.Text = "red";
					}
					/*else if (T2.Text == "blank")
						imageB1.SetImageResource(Resource.Id.button2); */

					else if (color == "Element 2")
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

		void DropZone_Drag2(object sender, View.DragEventArgs e)
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
					//result2.Text = "Drop it like it's hot!";

					if (T1.Text != "blank")
						
					T2.Text = T1.Text;

					if (T3.Text == T2.Text)
{
						image4.SetImageResource(Resource.Drawable.grey);
						T3.Text = "blank";
					}


					if (T3.Text != T4.Text)
						image4.SetImageResource(Resource.Drawable.grey);


					if (T2.Text == "red")
						imageB21.SetImageResource(Resource.Drawable.red);

					else if (T2.Text == "blue")
						imageB21.SetImageResource(Resource.Drawable.blue);




					

					break;
				// Dragged element exits the drop zone
				case DragAction.Exited:

					


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
					/*if (T1.Text == "red")
						{
						imageB21.SetImageResource(Resource.Drawable.red);
						T2.Text = "red";
					}

				else if (T1.Text == "blue")
					{
						imageB21.SetImageResource(Resource.Drawable.blue);
						T2.Text = "blue";
					}*/


					if (T2.Text == "red")
					{
						imageB21.SetImageResource(Resource.Drawable.grey);
						T2.Text = "blank";
					}

					else if (T2.Text == "blue")
					{
						imageB21.SetImageResource(Resource.Drawable.grey);
						T1.Text = "blank";
					}


					else if (color == "Element 1")
					{
						imageB21.SetImageResource(Resource.Drawable.red);
						T2.Text = "red";
					}
					/*else if (T2.Text == "blank")
						imageB1.SetImageResource(Resource.Id.button2); */

					else if (color == "Element 2")
					{
						imageB21.SetImageResource(Resource.Drawable.blue);
						T2.Text = "blue";
					}

					break;
			}
		}

		void DropZone_Drag3(object sender, View.DragEventArgs e)
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
					//result2.Text = "Drop it like it's hot!";





					T3.Text = T2.Text;

					if (T4.Text == T3.Text)
					{
						imageB4.SetImageResource(Resource.Drawable.grey);
						T4.Text = "blank";
					}






					if (T3.Text == "red")
						image4.SetImageResource(Resource.Drawable.red);
					else if (T3.Text == "blue")
						image4.SetImageResource(Resource.Drawable.blue);



					break;
				// Dragged element exits the drop zone
				case DragAction.Exited:
					var data2 = e.Event.ClipData;
					//if (data2.GetItemAt(2).Text == "red")

					break;
				// Dragged element has been dropped at the drop zone
				case DragAction.Drop:
					// You can check if element may be dropped here
					// If not do not set e.Handled to true
					e.Handled = true;

					// Try to get clip data
					var data = e.Event.ClipData;
					if (data != null)
						hold.Text = "11";
					break;
			}
		}


		void DropZone_Drag4(object sender, View.DragEventArgs e)
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

					T4.Text = T3.Text;

					/*if (T4.Text == T3.Text)
					{
						imageB4.SetImageResource(Resource.Drawable.grey);
						T4.Text = "blank";
					}*/






					if (T4.Text == "red")
						imageB4.SetImageResource(Resource.Drawable.red);
					if (T4.Text == "blue")
						imageB4.SetImageResource(Resource.Drawable.blue);


					break;
				// Dragged element exits the drop zone
				case DragAction.Exited:


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
					var image = FindViewById<ImageButton>(Resource.Id.myButton4);

					if (color == "Element 1")
					{
						imageB1.SetImageResource(Resource.Drawable.red);
						T4.Text = "red";
					}
					/*else if (T2.Text == "blank")
						imageB1.SetImageResource(Resource.Id.button2); */

					else if (color == "Element 2")
					{
						image.SetImageResource(Resource.Drawable.blue);
						T4.Text = "blue";
					}


					break;
			}
		}

	}
}
