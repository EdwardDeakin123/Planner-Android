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
using Android.Provider;
using Java.Util;
using Android.Util;





namespace DragAndDropDemo
{
	[Activity(Label = "EventListActivity")]
	public class Notification : Activity
	{
		int _calId;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);



			 _calId = 1;

			ListEvents();
			ListEvents2();

			//string Tag = Convert.ToString(_calId);
			//Log.Debug(Tag, "hello");
		}


		long GetDateTimeMS(int yr, int month, int day, int hr, int min)
		{
			Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);

			c.Set(Calendar.DayOfMonth, DateTime.Now.Day);
			c.Set(Calendar.HourOfDay, 20);
			c.Set(Calendar.Minute, 55);
			c.Set(Calendar.Month, DateTime.Now.Month - 1);
			c.Set(Calendar.Year, DateTime.Now.Year);

			return c.TimeInMillis;
		}

		long GetDateTimeMS2(int yr, int month, int day, int hr, int min)
		{
			Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);

			c.Set(Calendar.DayOfMonth, DateTime.Now.Day );
			c.Set(Calendar.HourOfDay, 20);
			c.Set(Calendar.Minute, 56);
			c.Set(Calendar.Month, DateTime.Now.Month - 1);
			c.Set(Calendar.Year, DateTime.Now.Year);

			return c.TimeInMillis;
		}



		void ListEvents()
		{
			var eventsUri = CalendarContract.Events.ContentUri;

			ContentValues eventValues = new ContentValues();
			eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, _calId);
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Title, "test1");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Description, "This is an event created from Mono for Android");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMS(0, 0, 0, 0, 0));
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMS(0, 0, 0, 0, 0));
			eventValues.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, "UTC");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "UTC");
			var uri = ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);
			Console.WriteLine("Uri for new event: {0}", uri);

			long eventID = long.Parse(uri.LastPathSegment);

				var reminderValues = new ContentValues();
				reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventID);
				reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Method, (int)RemindersMethod.Alert);
				reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 5);
			    
				var reminderUri = ContentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues);
				Console.WriteLine("Uri for new event: {0}", reminderUri);

			}
	

		void ListEvents2()
		{
			var eventsUri = CalendarContract.Events.ContentUri;



			ContentValues eventValues = new ContentValues();
			eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, _calId);
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Title, "test2");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Description, "This is an event created from Mono for Android");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMS2(0, 0, 0, 0, 0));
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMS2(0, 0, 0, 0, 0));
			eventValues.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, "UTC");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "UTC");
			var uri = ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);
			Console.WriteLine("Uri for new event: {0}", uri);

			long eventID = long.Parse(uri.LastPathSegment);

			var reminderValues = new ContentValues();
			reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventID);
			reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Method, (int)RemindersMethod.Alert);
			reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 5);

			var reminderUri = ContentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues);
			Console.WriteLine("Uri for new event: {0}", reminderUri);



		}


               





	}
}

