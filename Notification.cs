using System;
using Android.App;
using Android.Content;
using Android.Provider;
using Java.Util;
using Android.Util;
using Front_End.Database;
using Front_End.Models;
using Android.Database.Sqlite;

namespace Front_End
{
    public class Notification
	{
		int _calId;
        TimesDatabase _TimesDb;


		public Notification()
		{
			_calId = 1;

            _TimesDb = new TimesDatabase();

			ListEvents();
			ListEvents2();
		}

		long GetDateTimeMS(int yr, int month, int day, int hr, int min)
		{
			Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);

			c.Set(Java.Util.CalendarField.DayOfWeek, DateTime.Now.Day);
			c.Set(Java.Util.CalendarField.HourOfDay, 21);
			c.Set(Java.Util.CalendarField.Minute, 0);
			c.Set(Java.Util.CalendarField.Month, DateTime.Now.Month - 1);
			c.Set(Java.Util.CalendarField.Year, DateTime.Now.Year);

			return c.TimeInMillis;
		}

		long GetDateTimeMS2(int yr, int month, int day, int hr, int min)
		{
			Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);

			c.Set(Java.Util.CalendarField.DayOfWeek, DateTime.Now.Day + 1);
			c.Set(Java.Util.CalendarField.HourOfDay, 9);
			c.Set(Java.Util.CalendarField.Minute, 0);
			c.Set(Java.Util.CalendarField.Month, DateTime.Now.Month - 1);
			c.Set(Java.Util.CalendarField.Year, DateTime.Now.Year);

			return c.TimeInMillis;
		}

		void ListEvents()
		{
			var eventsUri = CalendarContract.Events.ContentUri;

			ContentValues eventValues = new ContentValues();
			eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, _calId);
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Title, "Have you filled in your planner today?");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Description, "Planner");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMS(0, 0, 0, 0, 0));
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMS(0, 0, 0, 0, 0));
			eventValues.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, "UTC");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "UTC");

			var uri = Application.Context.ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);
			Console.WriteLine("Uri for new event: {0}", uri);

			long eventID = long.Parse(uri.LastPathSegment);

			string Tags = eventID.ToString();

			Console.WriteLine("Here it is: {0}", Tags);

			var reminderValues = new ContentValues();
			reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventID);
			reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Method, (int)RemindersMethod.Alert);
			reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 1);

            try
            {
                var reminderUri = Application.Context.ContentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues);
                Console.WriteLine("Uri for new event: {0}", reminderUri);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Hit an error: " + e.Message);
            }
			
		}

		void ListEvents2()
		{
			var eventsUri = CalendarContract.Events.ContentUri;

			ContentValues eventValues = new ContentValues();
			eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, _calId);
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Title, "Please remember to fill in your planner today");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Description, "Planner");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMS2(0, 0, 0, 0, 0));
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMS2(0, 0, 0, 0, 0));
			eventValues.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, "UTC");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "UTC");

            try
            {
                var uri = Application.Context.ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);

                Console.WriteLine("Uri for new event: {0}", uri);

                long eventID = long.Parse(uri.LastPathSegment);
                string Tags = eventID.ToString();
                Console.WriteLine("Here it is:D {0}", Tags);

                var reminderValues = new ContentValues();
                reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventID);
                reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Method, (int)RemindersMethod.Alert);
                reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 1);

                var reminderUri = Application.Context.ContentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues);
                Console.WriteLine("Uri for new event: {0}", reminderUri);
            }
            catch (SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine("Hit an error: " + ex.Message);
            }

            _TimesDb.InsertData(new Times { Time = DateTime.Now.Day });
		}
	}
}