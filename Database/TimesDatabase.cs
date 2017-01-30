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
            try
            {
                // this counts all records in the database, it can be slow depending on the size of the database
                Times result = _Database.FindWithQuery<Times>("SELECT * FROM Times WHERE ID = (SELECT MAX(ID) FROM Times)");

                if(result == default(Times))
                {
                    // There are no existing elements in the database.
                    return -1;
                }

                return result.Time;
            }
            catch(SQLiteException)
            {
                // Something went wrong.
                return -1;
            }
        }
    }
}