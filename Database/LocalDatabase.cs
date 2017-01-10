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
using SQLite;

namespace Front_End.Database
{
    class LocalDatabase
    {
        private const string DATABASE_NAME = "db_sqlnet1.db";

        public SQLiteConnection GetDatabaseConnection()
        {
            string docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string pathToDatabase = System.IO.Path.Combine(docsFolder, DATABASE_NAME);

            // Return the database connection
            return new SQLiteConnection(pathToDatabase);
        }
    }
}