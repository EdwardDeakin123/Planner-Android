
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

using SQLite;





using Android.Util;
namespace Front_End
{
	[Activity(Label = "EventListActivity", MainLauncher = true)]
	public class EventListActivity : Activity
	{
		int _calId;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Main);

			_calId = 1;

			ListEvents();
			//ListEvents2();

			var text = FindViewById<TextView>(Resource.Id.TextTime);

			var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet1.db");

			var result = createDatabase(pathToDatabase);


			var result1 = insertUpdateData(new Times { Time = DateTime.Now.Day.ToString() }, pathToDatabase);
			var records = findTime(pathToDatabase);
			text.Text += records;
			string check = records;

			string Tag = Convert.ToString(check);
			Log.Debug(check, "hello");

			text.Text = records;


		}

		private string createDatabase(string path)
		{
			try
			{
				var connection = new SQLiteConnection(path);
				connection.CreateTable<Times>();
				return "Database created";
			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}

		private string insertUpdateData(Times data, string path)
		{
			try
			{
				var db = new SQLiteConnection(path);
				if (db.Insert(data) != 0)
					db.Update(data);
				return "Single data file inserted or updated";
			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}

		private string findTime(string path)
		{
			try
			{
				var db = new SQLiteConnection(path);
				// this counts all records in the database, it can be slow depending on the size of the database
				var count = db.FindWithQuery<Times>("SELECT * FROM Times WHERE ID = (SELECT MAX(ID) FROM Times)").Time;



				return count;
			}
			catch (SQLiteException)
			{
				return ToString();
			}
		}


		long GetDateTimeMS(int yr, int month, int day, int hr, int min)
		{
			Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);

			c.Set(Calendar.DayOfMonth, DateTime.Now.Day);
			c.Set(Calendar.HourOfDay, DateTime.Now.Hour);
			c.Set(Calendar.Minute, DateTime.Now.Minute + 3);
			c.Set(Calendar.Month, DateTime.Now.Month - 1);
			c.Set(Calendar.Year, DateTime.Now.Year);

			return c.TimeInMillis;
		}

		long GetDateTimeMS2(int yr, int month, int day, int hr, int min)
		{
			Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);

			c.Set(Calendar.DayOfMonth, DateTime.Now.Day);
			c.Set(Calendar.HourOfDay, DateTime.Now.Hour);
			c.Set(Calendar.Minute, DateTime.Now.Minute + 13);
			c.Set(Calendar.Month, DateTime.Now.Month - 1);
			c.Set(Calendar.Year, DateTime.Now.Year);

			return c.TimeInMillis;
		}



	

		void ListEvents()
		{
			var eventsUri = CalendarContract.Events.ContentUri;

			var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

			var records = findTime(pathToDatabase);

			if (records != DateTime.Now.Day.ToString())
			{

				ContentValues eventValues = new ContentValues();
				eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, _calId);
				eventValues.Put(CalendarContract.Events.InterfaceConsts.Title, "we did it");
				eventValues.Put(CalendarContract.Events.InterfaceConsts.Description, "This is an event created from Mono for Android");
				eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMS(0, 0, 0, 0, 0));
				eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMS2(0, 0, 0, 0, 0));
				eventValues.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, "UTC");
				eventValues.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "UTC");
				var uri = ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);
				Console.WriteLine("Uri for new event: {0}", uri);

				long eventID = long.Parse(uri.LastPathSegment);

				string Tags = Convert.ToString(eventID);

				Console.WriteLine("Here it is: {0}", Tags);

				var reminderValues = new ContentValues();
				reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventID);
				reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Method, (int)RemindersMethod.Alert);
				reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 1);

				var reminderUri = ContentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues);
				Console.WriteLine("Uri for new event: {0}", reminderUri);

				insertUpdateData(new Times { Time = DateTime.Now.Day.ToString() }, pathToDatabase);
			}

			else
				return;
		}





		/*void ListEvents2()
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



			string Tags = Convert.ToString(eventID);

			Console.WriteLine("Here it is:D {0}", Tags);

			var reminderValues = new ContentValues();
			reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventID);
			reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Method, (int)RemindersMethod.Alert);
			reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 1);

			var reminderUri = ContentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues);
			Console.WriteLine("Uri for new event: {0}", reminderUri);

			StartActivity(typeof(Main));

		}
*/







	}
}



