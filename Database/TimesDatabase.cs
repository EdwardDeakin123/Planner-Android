using SQLite;
using Front_End.Models;

namespace Front_End.Database
{
    class TimesDatabase
    {
        private SQLiteConnection _Database;

        public TimesDatabase()
        {
            // Get the database connection
            LocalDatabase db = new LocalDatabase();
            _Database = db.GetDatabaseConnection();

            CreateDatabase();
        }

        private void CreateDatabase()
        {
            _Database.CreateTable<Times>();
        }

        public void InsertData(Times data)
        {
            _Database.Insert(data);
        }

        public int FindLatest()
        {
            // this counts all records in the database, it can be slow depending on the size of the database
            int time = _Database.FindWithQuery<Times>("SELECT * FROM Times WHERE ID = (SELECT MAX(ID) FROM Times)").Time;

            System.Diagnostics.Debug.WriteLine("Got the following day: " + time);

            return time;
        }
    }
}