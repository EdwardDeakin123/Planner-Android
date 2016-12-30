using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;




using Android.Provider;
using Java.Util;

namespace DragAndDropDemo
{
	[Activity(Label = "Drag and Drop", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{

		TextView hold;
		string color;
		TextView T1;
		TextView T2;
		TextView T3;
		TextView T4;
		TextView T5;
		TextView T6;
		TextView T7;
		TextView T8;
		TextView T9;
		TextView T10;
		TextView T11;
		TextView T12;
		TextView T13;
		TextView T14;
		TextView T15;
		TextView T16;



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
			T5 = FindViewById<TextView>(Resource.Id.T5);
			T6 = FindViewById<TextView>(Resource.Id.T6);
			T7 = FindViewById<TextView>(Resource.Id.T7);
			T8 = FindViewById<TextView>(Resource.Id.T8);
			T9 = FindViewById<TextView>(Resource.Id.T9);
			T10 = FindViewById<TextView>(Resource.Id.T10);
			T11 = FindViewById<TextView>(Resource.Id.T11);
			T12 = FindViewById<TextView>(Resource.Id.T12);
			T13 = FindViewById<TextView>(Resource.Id.T13);
			T14 = FindViewById<TextView>(Resource.Id.T14);
			T15 = FindViewById<TextView>(Resource.Id.T15);
			T16 = FindViewById<TextView>(Resource.Id.T16);



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





			var button1 = FindViewById<Button>(Resource.Id.button1);
			button1.LongClick += Button1_LongClick;
			var button2 = FindViewById<Button>(Resource.Id.button2);
			button2.LongClick += Button2_LongClick;


			var dropZone1 = FindViewById<RelativeLayout>(Resource.Id.dropz);
			var dropZone2 = FindViewById<RelativeLayout>(Resource.Id.dropz1);
			var dropZone3 = FindViewById<RelativeLayout>(Resource.Id.dropz2);
			var dropZone4 = FindViewById<RelativeLayout>(Resource.Id.dropz3);
			var dropZone5 = FindViewById<RelativeLayout>(Resource.Id.dropz4);
			var dropZone6 = FindViewById<RelativeLayout>(Resource.Id.dropz5);
			var dropZone7 = FindViewById<RelativeLayout>(Resource.Id.dropz6);
			var dropZone8 = FindViewById<RelativeLayout>(Resource.Id.dropz7);
			var dropZone9 = FindViewById<RelativeLayout>(Resource.Id.dropz8);
			var dropZone10 = FindViewById<RelativeLayout>(Resource.Id.dropz9);
			var dropZone11 = FindViewById<RelativeLayout>(Resource.Id.dropz10);
			var dropZone12 = FindViewById<RelativeLayout>(Resource.Id.dropz11);
			var dropZone13 = FindViewById<RelativeLayout>(Resource.Id.dropz12);
			var dropZone14 = FindViewById<RelativeLayout>(Resource.Id.dropz13);
			var dropZone15 = FindViewById<RelativeLayout>(Resource.Id.dropz14);
			var dropZone16 = FindViewById<RelativeLayout>(Resource.Id.dropz15);




			imageB1.LongClick += IMButton1_LongClick;
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
			imageB16.LongClick += IMButton16_LongClick;


			// Attach event to drop zone
			dropZone1.Drag += DropZone_Drag;
			dropZone2.Drag += DropZone_Drag2;
			dropZone3.Drag += DropZone_Drag3;
			dropZone4.Drag += DropZone_Drag4;
			dropZone5.Drag += DropZone_Drag5;
			dropZone6.Drag += DropZone_Drag6;
			dropZone7.Drag += DropZone_Drag7;
		/*	dropZone4.Drag += DropZone_Drag8;
			dropZone1.Drag += DropZone_Drag9;
			dropZone2.Drag += DropZone_Drag10;
			dropZone3.Drag += DropZone_Drag11;
			dropZone4.Drag += DropZone_Drag12;
			dropZone1.Drag += DropZone_Drag13;
			dropZone2.Drag += DropZone_Drag14;
			dropZone3.Drag += DropZone_Drag15;
			dropZone4.Drag += DropZone_Drag16;*/




			base.OnCreate(bundle);
		}

		void IMButton1_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "1");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}

		void IMButton2_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "2");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}


		void IMButton3_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "3");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}


		void IMButton4_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "4");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}


		void IMButton5_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "5");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}


		void IMButton6_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "6");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}


		void IMButton7_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "7");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}


		void IMButton8_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "8");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}



		void IMButton9_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "9");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}


		void IMButton10_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "10");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}


		void IMButton11_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "11");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}


		void IMButton12_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "12");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}

		void IMButton13_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "13");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}

		void IMButton14_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "14");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}

		void IMButton15_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "15");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}

		void IMButton16_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "16");

			// Start dragging and pass data
			((sender) as ImageButton).StartDrag(data, new View.DragShadowBuilder(((sender) as ImageButton)), null, 0);
		}





		void Button1_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "red");

			// Start dragging and pass data
			((sender) as Button).StartDrag(data, new View.DragShadowBuilder(((sender) as Button)), null, 0);
		}

		void Button2_LongClick(object sender, View.LongClickEventArgs e)
		{
			// Generate clip data package to attach it to the drag
			var data = ClipData.NewPlainText("name", "blue");

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

					if (T2.Text == "blue")
					{
						T2.Text = T2.Text;
					}
				else if (T2.Text == "red")
					{
						T2.Text = T2.Text;
					}
						
					else if (T2.Text == "blank")

						T2.Text = T1.Text;

					if (T3.Text == T2.Text)
					{
						imageB3.SetImageResource(Resource.Drawable.grey);
						T3.Text = "blank";
					}


					if (T3.Text != T4.Text)
						imageB3.SetImageResource(Resource.Drawable.grey);


					if (T2.Text == "red")
						imageB2.SetImageResource(Resource.Drawable.red);

					else if (T2.Text == "blue")
						imageB2.SetImageResource(Resource.Drawable.blue);



					break;
					
				// Dragged element exits the drop zone
				case DragAction.Exited:

					if (T2.Text != T1.Text)
					{
						if (T2.Text == "blue")
							imageB2.SetImageResource(Resource.Drawable.blueup);

						else if (T2.Text == "red")
							imageB2.SetImageResource(Resource.Drawable.redup);
					}


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
					


				/*	if (T2.Text == "red")
					{
						imageB2.SetImageResource(Resource.Drawable.grey);
						T2.Text = "blank";
					}

					 if (T2.Text == "blue")
					{
						imageB2.SetImageResource(Resource.Drawable.grey);
						T2.Text = "blank";
					} */


					 if (color == "red")
					{
						imageB2.SetImageResource(Resource.Drawable.red);
						T2.Text = "red";
					}


				else if (color == "blue")
					{
						imageB2.SetImageResource(Resource.Drawable.blue);
						T2.Text = "blue";
					}

					else
					{
						T2.Text = "blank";
						imageB2.SetImageResource(Resource.Drawable.grey);

						if (T1.Text == "blue")
						{
							imageB1.SetImageResource(Resource.Drawable.bluedown);
						}

						else if (T1.Text == "red")
						{
							imageB1.SetImageResource(Resource.Drawable.reddown);
						}

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


					if (T3.Text == "blue")
					{
						T3.Text = T3.Text;
					}
					else if (T3.Text == "red")
					{
						T3.Text = T3.Text;
					}

					else if (T3.Text == "blank")

						T3.Text = T2.Text;

					if (T4.Text == T3.Text)
					{
						imageB4.SetImageResource(Resource.Drawable.grey);
						T4.Text = "blank";
					}


					if (T4.Text != T5.Text)
						imageB4.SetImageResource(Resource.Drawable.grey);


					if (T3.Text == "red")
						imageB3.SetImageResource(Resource.Drawable.red);

					else if (T3.Text == "blue")
						imageB3.SetImageResource(Resource.Drawable.blue);



					break;
				// Dragged element exits the drop zone
				case DragAction.Exited:
					var data2 = e.Event.ClipData;
					if (T3.Text != T2.Text)
					{
						if (T3.Text == "blue")
							imageB3.SetImageResource(Resource.Drawable.blueup);

						else if (T3.Text == "red")
							imageB3.SetImageResource(Resource.Drawable.redup);
					}


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
							imageB3.SetImageResource(Resource.Drawable.red);
							T3.Text = "red";
						}


						else if (color == "blue")
						{
							imageB3.SetImageResource(Resource.Drawable.blue);
							T3.Text = "blue";
						}

						else
						{
							T3.Text = "blank";
							imageB3.SetImageResource(Resource.Drawable.grey);

						if (T2.Text == "blue")
							{
								imageB2.SetImageResource(Resource.Drawable.bluedown);
						}

						else if (T2.Text == "red")
							{
								imageB2.SetImageResource(Resource.Drawable.reddown);
							}


						}
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

					if (T4.Text == "blue")
					{
						T4.Text = T4.Text;
					}
					else if (T4.Text == "red")
					{
						T4.Text = T4.Text;
					}

					else if (T4.Text == "blank")

						T4.Text = T3.Text;

					if (T5.Text == T4.Text)
					{
						imageB5.SetImageResource(Resource.Drawable.grey);
						T5.Text = "blank";
					}


					if (T5.Text != T6.Text)
						imageB5.SetImageResource(Resource.Drawable.grey);


					if (T4.Text == "red")
						imageB4.SetImageResource(Resource.Drawable.red);

					else if (T4.Text == "blue")
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





					if (color == "red")
					{
						imageB4.SetImageResource(Resource.Drawable.red);
						T4.Text = "red";
					}


					else if (color == "blue")
					{
						imageB4.SetImageResource(Resource.Drawable.blue);
						T4.Text = "blue";
					}

					else
					{
						T4.Text = "blank";
						imageB4.SetImageResource(Resource.Drawable.grey);

						if (T3.Text == "blue")
						{
							imageB3.SetImageResource(Resource.Drawable.bluedown);
						}

						else if (T3.Text == "red")
						{
							imageB3.SetImageResource(Resource.Drawable.reddown);
						}

					}


					break;
			}
		}

		void DropZone_Drag5(object sender, View.DragEventArgs e)
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

					if (T5.Text == "blue")
					{
						T5.Text = T5.Text;
					}
					else if (T5.Text == "red")
					{
						T5.Text = T5.Text;
					}

					else if (T5.Text == "blank")

						T5.Text = T4.Text;

					if (T6.Text == T5.Text)
					{
						imageB6.SetImageResource(Resource.Drawable.grey);
						T6.Text = "blank";
					}


					if (T6.Text != T7.Text)
						imageB6.SetImageResource(Resource.Drawable.grey);


					if (T5.Text == "red")
						imageB5.SetImageResource(Resource.Drawable.red);

					else if (T5.Text == "blue")
						imageB5.SetImageResource(Resource.Drawable.blue);



					break;

				// Dragged element exits the drop zone
				case DragAction.Exited:

					if (T5.Text != T4.Text)
					{
						if (T5.Text == "blue")
							imageB5.SetImageResource(Resource.Drawable.blueup);

						else if (T5.Text == "red")
							imageB5.SetImageResource(Resource.Drawable.redup);
					}


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
						imageB5.SetImageResource(Resource.Drawable.red);
						T5.Text = "red";
					}


					else if (color == "blue")
					{
						imageB5.SetImageResource(Resource.Drawable.blue);
						T5.Text = "blue";
					}

					else
					{
						T5.Text = "blank";
						imageB5.SetImageResource(Resource.Drawable.grey);

						if (T4.Text == "blue")
						{
							imageB4.SetImageResource(Resource.Drawable.bluedown);
						}

						else if (T4.Text == "red")
						{
							imageB4.SetImageResource(Resource.Drawable.reddown);
						}

					}



					break;
			}
		}

		void DropZone_Drag6(object sender, View.DragEventArgs e)
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

					if (T6.Text == "blue")
					{
						T6.Text = T6.Text;
					}
					else if (T6.Text == "red")
					{
						T6.Text = T6.Text;
					}

					else if (T6.Text == "blank")

						T6.Text = T5.Text;

					if (T7.Text == T6.Text)
					{
						imageB7.SetImageResource(Resource.Drawable.grey);
						T7.Text = "blank";
					}


					if (T7.Text != T8.Text)
						imageB7.SetImageResource(Resource.Drawable.grey);


					if (T6.Text == "red")
						imageB6.SetImageResource(Resource.Drawable.red);

					else if (T6.Text == "blue")
						imageB6.SetImageResource(Resource.Drawable.blue);



					break;

				// Dragged element exits the drop zone
				case DragAction.Exited:

					if (T6.Text != T5.Text)
					{
						if (T6.Text == "blue")
							imageB6.SetImageResource(Resource.Drawable.blueup);

						else if (T6.Text == "red")
							imageB6.SetImageResource(Resource.Drawable.redup);
					}


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
						imageB6.SetImageResource(Resource.Drawable.red);
						T6.Text = "red";
					}


					else if (color == "blue")
					{
						imageB6.SetImageResource(Resource.Drawable.blue);
						T6.Text = "blue";
					}

					else
					{
						T6.Text = "blank";
						imageB6.SetImageResource(Resource.Drawable.grey);

						if (T5.Text == "blue")
						{
							imageB5.SetImageResource(Resource.Drawable.bluedown);
						}

						else if (T5.Text == "red")
						{
							imageB5.SetImageResource(Resource.Drawable.reddown);
						}

					}



					break;
			}
		}

		void DropZone_Drag7(object sender, View.DragEventArgs e)
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

					if (T7.Text == "blue")
					{
						T7.Text = T7.Text;
					}
					else if (T7.Text == "red")
					{
						T7.Text = T7.Text;
					}

					else if (T7.Text == "blank")

						T7.Text = T6.Text;

					if (T8.Text == T7.Text)
					{
						imageB8.SetImageResource(Resource.Drawable.grey);
						T8.Text = "blank";
					}


					if (T8.Text != T9.Text)
						imageB8.SetImageResource(Resource.Drawable.grey);


					if (T7.Text == "red")
						imageB7.SetImageResource(Resource.Drawable.red);

					else if (T7.Text == "blue")
						imageB7.SetImageResource(Resource.Drawable.blue);



					break;

				// Dragged element exits the drop zone
				case DragAction.Exited:

					if (T7.Text != T6.Text)
					{
						if (T7.Text == "blue")
							imageB7.SetImageResource(Resource.Drawable.blueup);

						else if (T7.Text == "red")
							imageB7.SetImageResource(Resource.Drawable.redup);
					}


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
						imageB7.SetImageResource(Resource.Drawable.red);
						T7.Text = "red";
					}


					else if (color == "blue")
					{
						imageB7.SetImageResource(Resource.Drawable.blue);
						T7.Text = "blue";
					}

					else
					{
						T7.Text = "blank";
						imageB7.SetImageResource(Resource.Drawable.grey);

						if (T6.Text == "blue")
						{
							imageB6.SetImageResource(Resource.Drawable.bluedown);
						}

						else if (T6.Text == "red")
						{
							imageB6.SetImageResource(Resource.Drawable.reddown);
						}

					}



					break;
			}
		}
	}
}
